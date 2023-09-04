using Microsoft.EntityFrameworkCore;
using VM.Core.Constants;
using VM.Core.Exceptions;
using VM.Core.Extensions;
using VM.Data.Models;
using VM.Service.CurrencyService.ResponseModels;
using VM.Service.HttpService;

namespace VM.Service.CurrencyService
{
    public class BRLCurrencyService : CurrencyService
    {
        private readonly IHttpService _httpService;
        private readonly VM_DbContext _context;

        public BRLCurrencyService(IHttpService httpService, VM_DbContext context) : base(httpService, context)
        {
            _httpService = httpService;
            _context = context;
        }

        public override async Task<ExchangeRateResponse> GetExchangeRate(string isoCode)
        {
            if (string.IsNullOrEmpty(ApiURLs.BRL))
            {
                var response = await _httpService.GetAsync(ApiURLs.USD);

                if (response == null)
                {
                    throw new Exception();
                }

                var reponseToken = response.Split(',');
                var result = new ExchangeRateResponse
                {
                    Buy = Convert.ToDecimal(reponseToken[0].RemoveStringCharacters()),
                    Sell = Convert.ToDecimal(reponseToken[1].RemoveStringCharacters())
                };

                result.Buy /= 4;
                result.Sell /= 4;

                return result;
            } else
            {
                // TODO Implement Third Party endpoint call.
                throw new NotFoundException();
            }
        }

        public override async Task<bool> ValidatePurchaseLimit(int userId, string isoCode, decimal amount, decimal requestedAmountToPurchase)
        {
            var totalPurchaseByUserCurrentMonth = await _context.UserPurchases
                .Where(w => w.UserId == userId
                    && w.CurrencyIsoCode == isoCode
                    && w.PurchaseDate.Month == DateTime.Now.Month
                    && w.PurchaseDate.Year == DateTime.Now.Year)
                .ToListAsync();
            if (totalPurchaseByUserCurrentMonth.Sum(s => s.Amount) + requestedAmountToPurchase > PurchaseLimit.BRL)
            {
                throw new BadRequestException($"Monthly Amount {PurchaseLimit.BRL} Purchase Limit Exceeded.");
            }
            return true;
        }
    }
}
