namespace VM.Service.User
{
    public interface IUserService
    {
        Task<List<Data.Models.User>> GetAll();
    }
}
