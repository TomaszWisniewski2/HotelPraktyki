
using HotelProject.DAL.Entities;
using HotelProject.Services.AdminService;
using HotelProject.Services.AuthorizationService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HotelProject.Services.DTO;

namespace HotelProject.WEBAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthorizationController : Controller
    {
        private readonly Services.AuthorizationService.IAuthorizationService authorizationService;

        public AuthorizationController(Services.AuthorizationService.IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }
        [HttpPost("register")]
        public Task<ActionResult<User>> Register([FromBody] RegisterDTO request)=> authorizationService.Register(request);
        [HttpPost("Login")]
        public Task<TokenDTO> Login([FromBody] RegisterDTO request)=> authorizationService.Login(request);
        [HttpPut]
        public ActionResult<User> GetMe([FromBody] string token) => authorizationService.GetMe(token);
        //[HttpPost("AddRoomType")]
        //public Task<RoomTypeDTO> AddRoomType([FromBody] RoomTypeDTO dto) => adminService.AddRoomType(dto);
        //[HttpPost("AddRoom")]
        //public Task<RoomDTO> AddRoom([FromBody] RoomDTO dto) => adminService.AddRoom(dto);
        //[HttpPut("EditRoom")]
        //public Task<RoomDTO?> EditRoom([FromBody] RoomDTO dto) => adminService.EditRoom(dto);
        //[HttpPost("DeleteRoom")]
        //public Task<string> DeleteRoom([FromBody] RoomDTO dto) => adminService.DeleteRoom(dto);
        //[HttpGet("ListRoom")]
        //public Task<List<RoomDTO?>> ListRooms() => adminService.ListRooms();
        //[HttpGet("ListGuests")]
        //public Task<List<GuestDTO?>> ListGuests() => adminService.ListGuests();
        //[HttpGet("GetRoom")]
        //public Task<RoomDTO?> GetRoom(int id) => adminService.GetRoom(id);

    }
}