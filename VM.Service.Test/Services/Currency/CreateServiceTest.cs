namespace VM.Service.Test.Services.Currency
{
    public class CreateServiceTest
    {
        const string url = "https://www.bancoprovincia.com.ar/Principal/Dolar";

        [Fact]
        public void Create_Currency_Service_Required_Field_Exception()
        {
            var mockHttp = new MockHttpMessageHandler();
            var expectedExceptionType = typeof(RequiredFieldException);

            mockHttp.When(url).Respond("application/json", "[\"347.500\",\"365.500\",\"Actualizada al 30 / 8 / 2023 13:15\"]");

            var client = new HttpClient(mockHttp);
            var httpService = new HttpService.HttpService(client);
            var _currencyService = new CurrencyService.CurrencyService(httpService, null);
            
            var response = Assert.Throws<RequiredFieldException>(() => _currencyService.CreateService(""));
            var expectedMessage = $"Required field isoCode type {typeof(string)} is missing.";

            Assert.NotNull(response);
            Assert.IsType(expectedExceptionType, response);
            Assert.Equal(expectedMessage, response.Message);
        }

        [Fact]
        public void Create_Currency_Service_Invalid_Currency_Iso_Code()
        {
            var mockHttp = new MockHttpMessageHandler();
            var expectedExceptionType = typeof(InvalidCurrencyException);

            mockHttp.When(url).Respond("application/json", "[\"347.500\",\"365.500\",\"Actualizada al 30 / 8 / 2023 13:15\"]");

            var client = new HttpClient(mockHttp);
            var httpService = new HttpService.HttpService(client);
            var _currencyService = new CurrencyService.CurrencyService(httpService, null);

            var response = Assert.Throws<InvalidCurrencyException>(() => _currencyService.CreateService("TEST"));
            var expectedMessage = "Currency iso code TEST is not valid.";

            Assert.NotNull(response);
            Assert.IsType(expectedExceptionType, response);
            Assert.Equal(expectedMessage, response.Message);
        }

        [Fact]
        public void Create_Base_Currency_Service()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When(url).Respond("application/json", "[\"347.500\",\"365.500\",\"Actualizada al 30 / 8 / 2023 13:15\"]");

            var client = new HttpClient(mockHttp);
            var httpService = new HttpService.HttpService(client);
            var _currencyService = new CurrencyService.CurrencyService(httpService, null);

            var response = _currencyService.CreateService(CurrencyIsoCodes.USD);

            var expectedObjectType = typeof(CurrencyService.CurrencyService);

            Assert.NotNull(response);
            Assert.IsType(expectedObjectType, response);
        }

        [Fact]
        public void Create_BRL_Currency_Service()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When(url).Respond("application/json", "[\"347.500\",\"365.500\",\"Actualizada al 30 / 8 / 2023 13:15\"]");

            var client = new HttpClient(mockHttp);
            var httpService = new HttpService.HttpService(client);
            var _currencyService = new CurrencyService.CurrencyService(httpService, null);

            var response = _currencyService.CreateService(CurrencyIsoCodes.BRL);

            var expectedObjectType = typeof(CurrencyService.BRLCurrencyService);

            Assert.NotNull(response);
            Assert.IsType(expectedObjectType, response);
        }
    }
}
