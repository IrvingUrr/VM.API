using Microsoft.AspNetCore.Mvc;
using VM.Data.Models;
using VM.Service.User;

namespace VM.API.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        } 

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<User>>> GetAll()
        {
            return await _userService.GetAll();
        }
    }
}
