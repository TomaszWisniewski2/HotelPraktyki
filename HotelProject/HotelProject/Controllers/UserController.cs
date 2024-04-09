using HotelProject.Services.AdminService;
using HotelProject.Services.DTO;
using HotelProject.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace HotelProject.WEBAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsrController : Controller
    {
        private readonly IUserService userService;

        public UsrController(IUserService userService)
        {
            this.userService = userService;
        }


        [HttpPost("DeleteUser")]
        public  Task<string> DeleteUser(string token)=> userService.DeleteUser(token);

    }
}
