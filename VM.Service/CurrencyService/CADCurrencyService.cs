using VM.Data.Models;
using VM.Service.CurrencyService.ResponseModels;
using VM.Service.HttpService;

namespace VM.Service.CurrencyService
{
    internal class CADCurrencyService : CurrencyService
    {
        private readonly IHttpService _httpService;
        private readonly VM_DbContext _context;

        public CADCurrencyService(IHttpService httpService, VM_DbContext context) : base(httpService, context)
        {
            _httpService = httpService;
            _context = context;
        }

        public override async Task<ExchangeRateResponse> GetExchangeRate(string isoCode)
        {
            // TODO Implement Canadian Dollar Service.
            throw new NotImplementedException();
        }
    }
}
    