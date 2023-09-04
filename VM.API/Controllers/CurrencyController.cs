using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using VM.Core.Exceptions;
using VM.Service.CurrencyService;
using VM.Service.CurrencyService.ResponseModels;

namespace VM.API.Controllers
{
    [Route("[controller]")]
    public class CurrencyController : Controller
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet("{isoCode}/ExchangeRate")]
        [ProducesResponseType(typeof(ExchangeRateResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([Required] string isoCode)
        {
            var currencyService = _currencyService.CreateService(isoCode);
            var result = await currencyService.GetExchangeRate(isoCode);
            return Ok(result);
        }

        [HttpPost("{isoCode}/User/{userId}/Purchase/{amount}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([Required] string isoCode, [Required] int userId, [Required] decimal amount)
        {
            var currencyService = _currencyService.CreateService(isoCode);
            var result = await currencyService.RequestPurchase(userId, isoCode, amount);
            return Created(Request.GetEncodedUrl(), result);
        }
    }
}
