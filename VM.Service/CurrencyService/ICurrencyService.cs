using VM.Data.Models;
using VM.Service.CurrencyService.ResponseModels;

namespace VM.Service.CurrencyService
{
    public interface ICurrencyService
    {
        public CurrencyService CreateService(string isoCode);
        public Task<ExchangeRateResponse> GetExchangeRate(string isoCode);
        public Task<UserPurchase> RequestPurchase(int id, string isoCode, decimal amount);
    }
}
