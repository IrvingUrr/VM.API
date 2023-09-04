namespace VM.Service.Test.Services.Currency
{
    public class GetCurrencyExchangeRateTest
    {
        const string url = "https://www.bancoprovincia.com.ar/Principal/Dolar";

        [Fact]
        public void GetCurrencyExchangeRate_USD_Iso_Code()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(url)
                    .Respond("application/json", "[\"347.500\",\"365.500\",\"Actualizada al 30 / 8 / 2023 13:15\"]");

            var client = new HttpClient(mockHttp);
            var httpService = new HttpService.HttpService(client);
            var _currencyService = new CurrencyService.CurrencyService(httpService, null);
            var _currencyServiceUSD = _currencyService.CreateService(CurrencyIsoCodes.USD);

            var response = _currencyServiceUSD.GetExchangeRate(CurrencyIsoCodes.USD).Result;

            Assert.NotNull(response);
            Assert.Equal(347.500M, response.Buy);
            Assert.Equal(365.500M, response.Sell);
        }

        [Fact]
        public void GetCurrencyExchangeRate_USD_BRL_Iso_Code()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(url)
                    .Respond("application/json", "[\"347.500\",\"365.500\",\"Actualizada al 30 / 8 / 2023 13:15\"]");

            var client = new HttpClient(mockHttp);
            var httpService = new HttpService.HttpService(client);
            var _currencyService = new CurrencyService.CurrencyService(httpService, null);
            var _currencyServiceBRL = _currencyService.CreateService(CurrencyIsoCodes.BRL);

            var response = _currencyServiceBRL.GetExchangeRate(CurrencyIsoCodes.BRL).Result;

            Assert.NotNull(response);
            Assert.Equal(347.500M / 4, response.Buy);
            Assert.Equal(365.500M / 4, response.Sell);
        }
    }
}
