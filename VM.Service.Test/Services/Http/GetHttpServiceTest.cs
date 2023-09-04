namespace VM.Service.Test.Services.Http
{
    public class GetHttpServiceTest
    {
        const string url = "https://www.bancoprovincia.com.ar/Principal/Dolar";

        [Fact]
        public async void HttpService_Bad_Request()
        {
            var mockHttp = new MockHttpMessageHandler();
            var expectedExceptionType = typeof(BadRequestException);
            var message = "There was something wrong with your request.";
            var expectedException = new BadRequestException(message, url);

            mockHttp.When(url).Throw(expectedException);

            var client = new HttpClient(mockHttp);
            var httpService = new HttpService.HttpService(client);

            Func<Task> action = () => httpService.GetAsync(url);

            var response = await Assert.ThrowsAsync<BadRequestException>(action);

            Assert.NotNull(response);
            Assert.IsType(expectedExceptionType, response);
            Assert.Equal("There was something wrong with your request. Bad Request https://www.bancoprovincia.com.ar/Principal/Dolar", response.Message);
        }

        [Fact]
        public async void HttpService_Forbidden()
        {
            var mockHttp = new MockHttpMessageHandler();
            var expectedExceptionType = typeof(ForbiddenException);
            var message = "Forbidden Resource.";
            var expectedException = new ForbiddenException(message);

            mockHttp.When(url).Throw(expectedException);

            var client = new HttpClient(mockHttp);
            var httpService = new HttpService.HttpService(client);

            Func<Task> action = () => httpService.GetAsync(url);

            var response = await Assert.ThrowsAsync<ForbiddenException>(action);

            Assert.NotNull(response);
            Assert.IsType(expectedExceptionType, response);
            Assert.Equal(message, response.Message);
        }

        [Fact]
        public async void HttpService_Not_Found()
        {
            var mockHttp = new MockHttpMessageHandler();
            var expectedExceptionType = typeof(NotFoundException);
            var message = "Not found.";
            var expectedException = new NotFoundException(message);

            mockHttp.When(url).Throw(expectedException);

            var client = new HttpClient(mockHttp);
            var httpService = new HttpService.HttpService(client);

            Func<Task> action = () => httpService.GetAsync(url);

            var response = await Assert.ThrowsAsync<NotFoundException>(action);

            Assert.NotNull(response);
            Assert.IsType(expectedExceptionType, response);
            Assert.Equal(message, response.Message);
        }

        [Fact]
        public async void HttpService_Internal_Server_Error()
        {
            var mockHttp = new MockHttpMessageHandler();
            var expectedExceptionType = typeof(InternalServerErrorException);
            var message = "We are working to solve this as soon as possible.";
            var expectedException = new InternalServerErrorException(message);

            mockHttp.When(url).Throw(expectedException);

            var client = new HttpClient(mockHttp);
            var httpService = new HttpService.HttpService(client);

            Func<Task> action = () => httpService.GetAsync(url);

            var response = await Assert.ThrowsAsync<InternalServerErrorException>(action);

            Assert.NotNull(response);
            Assert.IsType(expectedExceptionType, response);
            Assert.Equal(message, response.Message);
        }

        [Fact]
        public async void HttpService_Success()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(url)
                    .Respond("application/json", "[\"347.500\",\"365.500\",\"Actualizada al 30 / 8 / 2023 13:15\"]");

            var client = new HttpClient(mockHttp);
            var httpService = new HttpService.HttpService(client);

            var response = await httpService.GetAsync(url);
            var expectedResponse = "[\"347.500\",\"365.500\",\"Actualizada al 30 / 8 / 2023 13:15\"]";

            Assert.NotNull(response);
            Assert.Equal(expectedResponse, response);
        }
    }
}
