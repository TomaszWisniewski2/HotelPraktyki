using HotelProject.Services.DTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelProject.Services.AdminService
{
    public interface IAdminService
    {
        Task<AddRoomDTO> AddRoom(AddRoomDTO dto);
        Task<string> DeleteRoom(RoomDTO dto, string token);
        Task<EditRoomDTO?> EditRoom(EditRoomDTO dto);
        Task<List<RoomDTO?>> ListRooms();
        Task<List<GuestDTO?>> ListGuests(string token);
        Task<RoomDTO?> GetRoom(int id, string token);
        Task<String> Payment(int bookingId, string token, decimal amount);
        Task<string> DeleteBooking(int bookingId, string token);
        Task<List<BookingDTO>> GetBookingList(string token);
        Task<BookingDTO> GetBooking(int bookingId, string token);
        Task<List<PaymentDTO>> GetPaymentList(string token);
        Task<PaymentDTO> GetPayment(int paymentId, string token);
        Task<BookingDTO> BookingRoom(AdminBookingDTO dto);
        Task<string> UpdatePhoto(IFormFile file, string token, int? roomId);
    }
}
