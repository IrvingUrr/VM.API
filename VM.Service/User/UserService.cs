using Microsoft.EntityFrameworkCore;
using VM.Data.Models;

namespace VM.Service.User
{
    public class UserService : IUserService
    {
        private readonly VM_DbContext _context;
        public UserService(VM_DbContext context)
        {
            _context = context;
        }

        public async Task<List<Data.Models.User>> GetAll()
        {
            return await _context.Users.Include(i => i.UserPurchases).ToListAsync();
        }
    }
}
