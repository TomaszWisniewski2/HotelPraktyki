using HotelProject.Common.Enums;
using HotelProject.DAL.Db_Access;
using HotelProject.DAL.Entities;
using HotelProject.Services.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HotelProject.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly Func<DatabaseContext> context;
        static JwtSecurityToken tokenOne = new JwtSecurityToken();

        public UserService(Func<DatabaseContext> context)
        {

            this.context = context;
        }
        public static void Decode(string token) //ActionResult<JwtSecurityToken>
        {
            if (token.Length > 0)
            {
                var stream = token;
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(stream);
                tokenOne = jsonToken as JwtSecurityToken;
            }
        }
        public async Task<String> Payment(string token)
        {
            using var ctx = context();
            Decode(token);
            var x = tokenOne;           
            var tokenValue = x.Payload.Select(kvp => kvp.Value).ToList();
            if (tokenValue[1].ToString() != "Admin") return null;
            //Booking? bookink = await ctx.Booking.FindAsync(id);
            //if (bookink == null) return "null";
            //bookink. = (Common.Enums.PaymentType)enu;
            //await ctx.SaveChangesAsync();
            //return "done";
            return null;
        }
        public async Task<BookingDTO?> GetBooking(int id)
        {
            using var ctx = context();
            var q = ctx.Booking.AsQueryable().AsNoTracking().Where(x => x.Id == id);
            var result = await q.Select(BookingAddEditDTOExpression).FirstAsync();

            return result;
        }
        public async Task<string> DeleteBooking(int bookingId, string token)
        {
            using var ctx = context();
            if (bookingId == 0) return "null";
            Decode(token);
            var x = tokenOne;
            var tokenValue = x.Payload.Select(kvp => kvp.Value).ToList();
            var userBookingId = await ctx.Booking.FindAsync(bookingId);
            if (Int32.Parse(tokenValue[2].ToString()) != userBookingId.UserId) return null;
            Booking? booking = await ctx.Booking.FindAsync(bookingId);
            if (booking == null) return "null";

            Room? room = await ctx.Room.FindAsync(booking.RoomId);
            if (room == null) return null;
            room.RoomStatus = RoomStatus.free;

            Payments? payments = await ctx.Payments.FindAsync(booking.Id);
            if (payments != null) ctx.Payments.Remove(payments);
            

            ctx.Booking.Remove(booking);
            await ctx.SaveChangesAsync();
            return "done";
        }

        public async Task<string> DeleteUser(string token)//dodać potwierdzenie hasłem
        {
            using var ctx = context();
            Decode(token);
            var x = tokenOne;
            var tokenValue = x.Payload.Select(kvp => kvp.Value).ToList();
            //if (tokenValue[1].ToString() != "Admin") return null;
            var userId = Int32.Parse((string)tokenValue[2]);
            User? user = await ctx.User.FindAsync(userId);
            if (user == null) return "null";
            var bookings =  ctx.Booking.Where(x=>x.UserId==userId).ToList();
            if (bookings != null)
            {
                foreach (var booking in bookings)
                {
                    Payments? payments = await ctx.Payments.FindAsync(booking.Id);
                    if (payments != null) ctx.Payments.Remove(payments);
                    ctx.Booking.Remove(booking);
                    await ctx.SaveChangesAsync();
                }
            }
            ctx.User.Remove(user);
            await ctx.SaveChangesAsync();
            return "done";
        }
        public async Task<EditBookingDTO> EditBooking(EditBookingDTO dto, string token)
        {
            using var ctx = context();
            if (!dto.Id.HasValue)
                return null;
            Booking? bookink = await ctx.Booking.FindAsync(dto.Id.Value);
            if (bookink == null) return null;
            BookingEdit(bookink, dto);
            await ctx.SaveChangesAsync();
#pragma warning disable CS8603 // Possible null reference return.
            return BookingEditDTOExpression.Invoke(bookink);
#pragma warning disable CS8603 // Possible null reference return.

        }
        public async Task<BookingDTO> BookingRoom(BookingDTO dto, string token)
        {
            using var ctx = context();
            Decode(token);
            var x = tokenOne;
            var tokenValue = x.Payload.Select(kvp => kvp.Value).ToList();
            var userId = Int32.Parse((string)tokenValue[2]);//id
            Booking booking = new();
            await ctx.Booking.AddAsync(booking);
            UpdateBooking(booking, dto, userId);
            await ctx.SaveChangesAsync();
            Room? room = await ctx.Room.FindAsync(dto.RoomId.Value);
            if (room == null) return null;
            room.RoomStatus = RoomStatus.reserved;
            await ctx.SaveChangesAsync();
#pragma warning disable CS8603 // Possible null reference return.
            return BookingAddEditDTOExpression.Invoke(booking);
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task<List<RoomDTO?>> ListRooms()
        {
            using var ctx = context();
            var q = ctx.Room.AsQueryable().AsNoTracking().Where(x => x.RoomStatus == 0);
            var result = await q.Select(RoomListDTOExpression).ToListAsync();

            return result;
        }


        //public async Task<List<GuestDTO?>> ListGuests()
        //{
        //    using var ctx = context();
        //    var q = ctx.User.AsQueryable().AsNoTracking();
        //    var result = await q.Select(GuestListDTOExpression).ToListAsync();

        //    return result;
        //}


        public async Task<GuestDTO?> EditGuest(GuestDTO dto, string token)
        {
            using var ctx = context();
            if (!dto.Id.HasValue)
                return null;
            User? guest = await ctx.User.FindAsync(dto.Id.Value);
            if (guest == null) return null;
            Update(guest, dto);
            await ctx.SaveChangesAsync();

            return GuestAddEditDTOExpression.Invoke(guest);

        }
        private static void Update(User guest, GuestDTO dto)
        {
            guest.Name = dto.Name ?? guest.Name;
            guest.Email = dto.Email ?? guest.Email;
            //guest.Password = dto.Password ?? guest.Password;
            guest.Surname = dto.Surname ?? guest.Surname;

        }
        private static void UpdateBooking(Booking booking, BookingDTO dto, int? userId)
        {
            booking.UserId= userId ?? booking.UserId;
            booking.RoomId = dto.RoomId ?? booking.RoomId;
            booking.BookingTo = dto.BookingTo ?? booking.BookingTo;
            booking.BookingFrom = dto.BookingFrom ?? booking.BookingFrom;

        }
        private static void BookingEdit(Booking booking, EditBookingDTO dto)
        {
            //booking.GuestId = dto.GuestId ?? booking.GuestId;
            booking.Id = dto.Id ?? booking.Id;
            booking.RoomId = dto.RoomId ?? booking.RoomId;
            booking.BookingTo = dto.BookingTo ?? booking.BookingTo;
            booking.BookingFrom = dto.BookingFrom ?? booking.BookingFrom;

        }
        
        #region Expressions
        internal static readonly Expression<Func<User, GuestDTO?>> GuestListDTOExpression = x => new GuestDTO()
        {
            Id = x.Id,
            Email = x.Email,
            //Password = x.Password,
            Surname = x.Surname,
            Name = x.Name
        };

        internal static readonly Expression<Func<User, GuestDTO?>> GuestAddEditDTOExpression = x => new GuestDTO()
        {
            Id = x.Id,
            Email = x.Email,
            //Password = x.Password,
            Surname = x.Surname,
            Name = x.Name
        };
        internal static readonly Expression<Func<Room, RoomDTO?>> RoomListDTOExpression = x => new RoomDTO()
        {
            Id = x.Id,
            RoomNumber = x.RoomNumber,
            RoomPrice = x.RoomPrice,
            RoomCapacity = x.RoomCapacity,
            RoomPhoto = x.RoomPhoto,
            //RoomStatus = x.RoomStatus,
            //RoomTypeId = x.RoomTypeId
        };
        internal static readonly Expression<Func<Booking, BookingDTO?>> BookingAddEditDTOExpression = x => new BookingDTO()
        {
            //GuestId = x.UserId,
            RoomId = x.RoomId,
            BookingFrom = x.BookingFrom,
            BookingTo = x.BookingTo
        };

        internal static readonly Expression<Func<Booking, EditBookingDTO?>> BookingEditDTOExpression = x => new EditBookingDTO()
        {

            RoomId = x.RoomId,
            BookingFrom = x.BookingFrom,
            BookingTo = x.BookingTo
        };
        #endregion

    }
}
