//using AppZJOHotel.Services.AdminService;
//using AppZJOHotel.Services.GuestService;
using HotelProject.Services.AdminService;
using HotelProject.Services.DTO;
using Microsoft.AspNetCore.Mvc;
using YamlDotNet.Core.Tokens;

//using AppZJOHotel.GuestServices;
namespace HotelProject.WEBAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IAdminService adminService;

        public AdminController(IAdminService adminService)
        {
            this.adminService = adminService;
        }

        
        [HttpPost("AddRoom")]
        public Task<AddRoomDTO> AddRoom([FromBody] AddRoomDTO dto) => adminService.AddRoom(dto);
        [HttpPost("EditRoom")]
        public Task<EditRoomDTO?> EditRoom([FromBody] EditRoomDTO dto) => adminService.EditRoom(dto);
        [HttpPost("DeleteRoom")]
        public Task<string> DeleteRoom([FromBody] RoomDTO dto, string token) => adminService.DeleteRoom(dto, token);
        [HttpGet("ListRoom")]
        public Task<List<RoomDTO?>> ListRooms() => adminService.ListRooms();
        [HttpGet("ListGuests")]
        public Task<List<GuestDTO?>> ListGuests([FromQuery] string token) => adminService.ListGuests(token);
        [HttpGet("GetRoom")]
        public Task<RoomDTO?> GetRoom([FromQuery] int id, string token) => adminService.GetRoom(id, token);
        [HttpPost("Pay")]
        public Task<String> Payment([FromBody] int bookingId, string token, decimal amount) => adminService.Payment(bookingId, token, amount);
        [HttpPost("Delete Booking")]
        public Task<string> DeleteBooking([FromBody] int bookingId, string token)=> adminService.DeleteBooking(bookingId, token);
        [HttpGet("GetBookingList")]
        public Task<List<BookingDTO>> GetBookingList([FromQuery] string token)=> adminService.GetBookingList(token);
        [HttpGet("GetBooking")]
        public Task<BookingDTO> GetBooking([FromQuery] int bookingId, string token)=> adminService.GetBooking(bookingId,token);
        [HttpGet("GetPaymentList")]
        public Task<List<PaymentDTO>> GetPaymentList([FromQuery] string token)=> adminService.GetPaymentList(token);
        [HttpGet("GetPayment")]
        public Task<PaymentDTO> GetPayment([FromQuery] int paymentId, string token)=> adminService.GetPayment(paymentId, token);
        [HttpPost("BookingRoom")]
        public Task<BookingDTO> BookingRoom([FromBody] AdminBookingDTO dto)=> adminService.BookingRoom(dto);
        [HttpPost("UpdatePhoto")]
        public Task<string> UpdatePhoto(IFormFile file, string token, int? roomId)=> adminService.UpdatePhoto(file, token, roomId);
    }
}