namespace VM.Service.Test.Services.Currency
{
    public class RequestPurchaseTest : IDisposable
    {
        public readonly VM_DbContext _context;
        const string url = "https://www.bancoprovincia.com.ar/Principal/Dolar";

        public RequestPurchaseTest()
        {
            _context = CreateDbContext();
        }
        
        [Fact]
        public async void RequestPurchase_Invalid_User()
        {
            var _currencyService = new CurrencyService.CurrencyService(null, _context);

            Func<Task> action = () => _currencyService.RequestPurchase(0, "", 0);
            var response = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            var expectedExceptionType = typeof(EntityNotFoundException);
            var expectedMessage = "Entity userId with Key: (0) was not found.";

            Assert.NotNull(response);
            Assert.IsType(expectedExceptionType, response);
            Assert.Equal(expectedMessage, response.Message);
        }

        [Fact]
        public async void RequestPurchase_Null_Or_Empty_Currency_IsoCode()
        {
            var _currencyService = new CurrencyService.CurrencyService(null, _context);

            Func<Task> action = () => _currencyService.RequestPurchase(1, "", 0);
            var response = await Assert.ThrowsAsync<RequiredFieldException>(action);

            var expectedExceptionType = typeof(RequiredFieldException);
            var expectedMessage = $"Required field isoCode type {typeof(string)} is missing.";

            Assert.NotNull(response);
            Assert.IsType(expectedExceptionType, response);
            Assert.Equal(expectedMessage, response.Message);
        }

        [Fact]
        public async void RequestPurchase_Invalid_Currency_IsoCode()
        {
            var _currencyService = new CurrencyService.CurrencyService(null, _context);

            Func<Task> action = () => _currencyService.RequestPurchase(1, "Test", 0);
            var response = await Assert.ThrowsAsync<InvalidCurrencyException>(action);

            var expectedExceptionType = typeof(InvalidCurrencyException);
            var expectedMessage = "Currency iso code TEST is not valid.";

            Assert.NotNull(response);
            Assert.IsType(expectedExceptionType, response);
            Assert.Equal(expectedMessage, response.Message);
        }

        [Theory]
        [InlineData(-100)]
        [InlineData(-1.5)]
        [InlineData(0)]
        public async void RequestPurchase_Invalid_Amount_Must_Be_Higher_Than_0(decimal amount)
        {
            var _currencyService = new CurrencyService.CurrencyService(null, _context);

            Func<Task> action = () => _currencyService.RequestPurchase(1, "USD", amount);
            var response = await Assert.ThrowsAsync<BadRequestException>(action);

            var expectedExceptionType = typeof(BadRequestException);
            var expectedMessage = "Purchase Amount Must Be Higher Than 0.";

            Assert.NotNull(response);
            Assert.IsType(expectedExceptionType, response);
            Assert.Equal(expectedMessage, response.Message);
        }

        [Theory]
        [InlineData(366)]
        [InlineData(500)]
        public async void RequestPurchase_Validate_Purchase_Limit_USD(decimal amount)
        {
            var userPurchase = new UserPurchase
            {
                Id = 1,
                UserId = 1,
                Amount = 199,
                CurrencyIsoCode = CurrencyIsoCodes.USD,
                PurchaseDate = DateTime.UtcNow,
            };

            _context.Add(userPurchase);
            await _context.SaveChangesAsync();

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(url)
                    .Respond("application/json", "[\"347.500\",\"365.500\",\"Actualizada al 30 / 8 / 2023 13:15\"]");

            var client = new HttpClient(mockHttp);
            var httpService = new HttpService.HttpService(client);

            var _currencyService = new CurrencyService.CurrencyService(httpService, _context);

            Func<Task> action = () => _currencyService.RequestPurchase(1, "USD", amount);
            var response = await Assert.ThrowsAsync<BadRequestException>(action);

            var expectedExceptionType = typeof(BadRequestException);
            var expectedMessage = $"Monthly Amount {PurchaseLimit.USD} Purchase Limit Exceeded.";

            Assert.NotNull(response);
            Assert.IsType(expectedExceptionType, response);
            Assert.Equal(expectedMessage, response.Message);
        }

        [Theory]
        [InlineData(92)]
        [InlineData(500)]
        public async void RequestPurchase_Validate_Purchase_Limit_BRL(decimal amount)
        {
            var userPurchase = new UserPurchase
            {
                Id = 1,
                UserId = 1,
                Amount = 299,
                CurrencyIsoCode = CurrencyIsoCodes.BRL,
                PurchaseDate = DateTime.UtcNow,
            };

            _context.Add(userPurchase);
            await _context.SaveChangesAsync();

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(url)
                    .Respond("application/json", "[\"347.500\",\"365.500\",\"Actualizada al 30 / 8 / 2023 13:15\"]");

            var client = new HttpClient(mockHttp);
            var httpService = new HttpService.HttpService(client);

            var _currencyService = new CurrencyService.BRLCurrencyService(httpService, _context);

            Func<Task> action = () => _currencyService.RequestPurchase(1, "BRL", amount);
            var response = await Assert.ThrowsAsync<BadRequestException>(action);

            var expectedExceptionType = typeof(BadRequestException);
            var expectedMessage = $"Monthly Amount {PurchaseLimit.BRL} Purchase Limit Exceeded.";

            Assert.NotNull(response);
            Assert.IsType(expectedExceptionType, response);
            Assert.Equal(expectedMessage, response.Message);
        }

        [Fact]
        public async void RequestPurchase_Valid_Request_Purchase_USD()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(url)
                    .Respond("application/json", "[\"347.500\",\"365.500\",\"Actualizada al 30 / 8 / 2023 13:15\"]");

            var client = new HttpClient(mockHttp);
            var httpService = new HttpService.HttpService(client);

            var _currencyService = new CurrencyService.CurrencyService(httpService, _context);

            var response = await _currencyService.RequestPurchase(1, "USD", 500);

            Assert.NotNull(response);
            Assert.Equal(1, response.UserId);
            Assert.Equal("USD", response.CurrencyIsoCode);
            Assert.Equal(1.368M, response.Amount);
        }

        [Fact]
        public async void RequestPurchase_Valid_Request_Purchase_BRL()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(url)
                    .Respond("application/json", "[\"347.500\",\"365.500\",\"Actualizada al 30 / 8 / 2023 13:15\"]");

            var client = new HttpClient(mockHttp);
            var httpService = new HttpService.HttpService(client);

            var _currencyService = new CurrencyService.BRLCurrencyService(httpService, _context);

            var response = await _currencyService.RequestPurchase(1, "BRL", 500);

            Assert.NotNull(response);
            Assert.Equal(1, response.UserId);
            Assert.Equal("BRL", response.CurrencyIsoCode);
            Assert.Equal(5.472M, response.Amount);
        }

        [Fact]
        public async void RequestPurchase_Valid_Request_Purchase_USD_Last_Month_Exceeded()
        {
            var userPurchase = new UserPurchase
            {
                Id = 1,
                UserId = 1,
                Amount = 199,
                CurrencyIsoCode = CurrencyIsoCodes.USD,
                PurchaseDate = new DateTime(2020, 1, 15),
            };

            _context.Add(userPurchase);
            await _context.SaveChangesAsync();

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(url)
                    .Respond("application/json", "[\"347.500\",\"365.500\",\"Actualizada al 30 / 8 / 2023 13:15\"]");

            var client = new HttpClient(mockHttp);
            var httpService = new HttpService.HttpService(client);

            var _currencyService = new CurrencyService.CurrencyService(httpService, _context);

            var response = await _currencyService.RequestPurchase(1, "USD", 500);

            Assert.NotNull(response);
            Assert.Equal(1, response.UserId);
            Assert.Equal("USD", response.CurrencyIsoCode);
            Assert.Equal(1.368M, response.Amount);
        }

        [Fact]
        public async void RequestPurchase_Valid_Request_Purchase_BRL_Last_Month_Exceeded()
        {
            var userPurchase = new UserPurchase
            {
                Id = 1,
                UserId = 1,
                Amount = 299,
                CurrencyIsoCode = CurrencyIsoCodes.BRL,
                PurchaseDate = new DateTime(2020, 1, 15),
            };

            _context.Add(userPurchase);
            await _context.SaveChangesAsync();

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(url)
                    .Respond("application/json", "[\"347.500\",\"365.500\",\"Actualizada al 30 / 8 / 2023 13:15\"]");

            var client = new HttpClient(mockHttp);
            var httpService = new HttpService.HttpService(client);

            var _currencyService = new CurrencyService.BRLCurrencyService(httpService, _context);

            var response = await _currencyService.RequestPurchase(1, "BRL", 500);

            Assert.NotNull(response);
            Assert.Equal(1, response.UserId);
            Assert.Equal("BRL", response.CurrencyIsoCode);
            Assert.Equal(5.472M, response.Amount);
        }

        private VM_DbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<VM_DbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            var dbContext = new VM_DbContext(options); dbContext.Users.AddRange(GetFakeData().AsQueryable());
            dbContext.SaveChanges();
            return dbContext;
        }
        private List<Data.Models.User> GetFakeData()
        {
            var users = new List<Data.Models.User>
              {
                new Data.Models.User { Id = 1, Name = "Test User" }
              };
            return users;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
