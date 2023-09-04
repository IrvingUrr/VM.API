namespace VM.Service.HttpService
{
    public interface IHttpService
    {
        Task<string> GetAsync(string uri);
    }
}
