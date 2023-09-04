using System.Text;
using VM.Core.Exceptions;

namespace VM.Service.HttpService
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _client;

        public HttpService(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> GetAsync(string uri)
        {
            using HttpResponseMessage response = await _client.GetAsync(uri);

            ValidateResponse(response, uri);

            return await response.Content.ReadAsStringAsync();
        }

        private void ValidateResponse(HttpResponseMessage response, string uri)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                throw new BadRequestException("There was something wrong with your request.", uri);
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                throw new ForbiddenException("Forbidden Resource.");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                throw new NotFoundException("Not found.");
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                throw new InternalServerErrorException("We are working to solve this as soon as possible.");
        }
    }
}
