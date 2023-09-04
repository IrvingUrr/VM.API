using Microsoft.EntityFrameworkCore;
using VM.Core.Constants;
using VM.Core.Exceptions;
using VM.Core.Extensions;
using VM.Data.Models;
using VM.Service.CurrencyService.ResponseModels;
using VM.Service.HttpService;

namespace VM.Service.CurrencyService
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IHttpService _httpService;
        private readonly VM_DbContext _context;

        public CurrencyService(IHttpService httpService, VM_DbContext context)
        {
            _httpService = httpService;
            _context = context;
        }

        public CurrencyService CreateService(string isoCode)
        {
            ValidateIsNullOrEmptyIsoCode(isoCode);

            return isoCode.ToUpper() switch
            {
                CurrencyIsoCodes.USD => this,
                CurrencyIsoCodes.BRL => new BRLCurrencyService(_httpService, _context),
                _ => throw new InvalidCurrencyException($"Currency iso code {isoCode.ToUpper()} is not valid."),
            };
        }

        public virtual async Task<ExchangeRateResponse> GetExchangeRate(string isoCode)
        {
            var response = await _httpService.GetAsync(ApiURLs.USD);

            var reponseToken = response.Split(',');
            var result = new ExchangeRateResponse
            {
                Buy = Convert.ToDecimal(reponseToken[0].RemoveStringCharacters()),
                Sell = Convert.ToDecimal(reponseToken[1].RemoveStringCharacters())
            };

            return result;
        }

        public async Task<UserPurchase> RequestPurchase(int userId, string isoCode, decimal amount)
        {
            await ValidateUserExist(userId);
            ValidateIsNullOrEmptyIsoCode(isoCode);
            ValidateExistentIsoCode(isoCode);
            ValidateAmount(amount);

            var isoCodeToUpper = isoCode.ToUpper();
            var exchangeRate = await GetExchangeRate(isoCode);
            var requestedAmountToPurchase = Math.Round(amount / exchangeRate.Sell, 3);
            await ValidatePurchaseLimit(userId, isoCodeToUpper, amount, requestedAmountToPurchase);

            var newUserPurchase = new UserPurchase
            {
                UserId = userId,
                Amount = requestedAmountToPurchase,
                CurrencyIsoCode = isoCodeToUpper,
                PurchaseDate = DateTime.UtcNow
            };

            _context.Add(newUserPurchase);
            await _context.SaveChangesAsync();
            
            return newUserPurchase;
        }

        private async Task<bool> ValidateUserExist(int userId)
        {
            var userExist = await _context.Users.AnyAsync(a => a.Id == userId);
            if (!userExist)
            {
                throw new EntityNotFoundException(nameof(userId), userId);
            }
            return true;
        }

        private static bool ValidateIsNullOrEmptyIsoCode(string isoCode)
        {
            if (string.IsNullOrEmpty(isoCode))
            {
                throw new RequiredFieldException(typeof(string), nameof(isoCode));
            }
            return true;
        }

        private static bool ValidateExistentIsoCode(string isoCode)
        {
            if (!isoCode.ToUpper().Equals(CurrencyIsoCodes.USD) && !isoCode.ToUpper().Equals(CurrencyIsoCodes.BRL))
            {
                throw new InvalidCurrencyException($"Currency iso code {isoCode.ToUpper()} is not valid.");
            }
            return true;
        }

        private static bool ValidateAmount(decimal amount)
        {
            if (amount <= 0)
            {
                throw new BadRequestException("Purchase Amount Must Be Higher Than 0.");
            }
            return true;
        }

        public virtual async Task<bool> ValidatePurchaseLimit(int userId, string isoCode, decimal amount, decimal requestedAmountToPurchase)
        {
            var totalPurchaseByUserCurrentMonth = await _context.UserPurchases
                .Where(w => w.UserId == userId 
                    && w.CurrencyIsoCode == isoCode
                    && w.PurchaseDate.Month == DateTime.Now.Month 
                    && w.PurchaseDate.Year == DateTime.Now.Year)
                .ToListAsync();
            if (totalPurchaseByUserCurrentMonth.Sum(s => s.Amount) + requestedAmountToPurchase > PurchaseLimit.USD)
            {
                throw new BadRequestException($"Monthly Amount {PurchaseLimit.USD} Purchase Limit Exceeded.");
            }
            return true;
        }
    }
}
