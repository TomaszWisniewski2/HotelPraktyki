using HotelProject.Common.Enums;
using HotelProject.DAL.Db_Access;
using HotelProject.DAL.Entities;
using HotelProject.Services.DTO;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HotelProject.Services.AdminService
{

    public class AdminService : IAdminService
    {
        private readonly Func<DatabaseContext> context;
        static JwtSecurityToken tokenOne = new JwtSecurityToken();

        public AdminService(Func<DatabaseContext> context)
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
        public async Task<string> UpdatePhoto(IFormFile file, string token, int? roomId)
        {
            if (file==null) { return "file is null"; }
            using var ctx = context();
            string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            string path = @"HotelProjectAngular\src\assets";
            string newPath = Path.GetFullPath(Path.Combine(@"..\..\", path));
            // Określenie ścieżki do katalogu photos
            string photoDirectory = Path.Combine(newPath, "photos");//AppDomain.CurrentDomain.BaseDirectory

            // Sprawdzenie istnienia katalogu i utworzenie go, jeśli nie istnieje
            if (!Directory.Exists(photoDirectory))
            {
                Directory.CreateDirectory(photoDirectory);
            }

            // Utworzenie pełnej ścieżki do zapisu pliku
            string filePath = Path.Combine(photoDirectory, uniqueFileName);

            // Zapis pliku na dysku
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            Room room = await ctx.Room.FindAsync(roomId);
            room.RoomPhoto = uniqueFileName;
            await ctx.SaveChangesAsync();

            // Zwrócenie nazwy zapisanego pliku
            return uniqueFileName;
            //------
         
        }
        public async Task<string> DeleteBooking(int bookingId, string token)
        {
            using var ctx = context();
            if (bookingId == null) return "null";
            Decode(token);
            var x = tokenOne;
            var tokenValue = x.Payload.Select(kvp => kvp.Value).ToList();
            var userBookingId = await ctx.Booking.FindAsync(bookingId);
            if (tokenValue[1].ToString() != "Admin") return null;
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
        public async Task<RoomDTO?> GetRoom(int id, string token)
        {
            using var ctx = context();
            Decode(token);
            var x = tokenOne;
            var y = x.Payload.First();
            var tokenValue = x.Payload.Select(kvp => kvp.Value).ToList();

            if (id == null) return null;
            if (x == null) return null;
            if (tokenValue[1].ToString() != "Admin") return null;

            Room? room = await ctx.Room.FindAsync(id);
            if (room == null) return null;
#pragma warning disable CS8603 // Possible null reference return.
            return RoomAddEditDTOExpression.Invoke(room);
#pragma warning disable CS8603 // Possible null reference return.
        }
        public async Task<List<RoomDTO?>> ListRooms()
        {
            using var ctx = context();
            var q = ctx.Room.AsQueryable().AsNoTracking();
            var result = await q.Select(RoomListDTOExpression).ToListAsync();

            return result;
        }
        public async Task<List<GuestDTO?>> ListGuests(string token)
        {
            using var ctx = context();
            Decode(token);
            var x = tokenOne;
            var y = x.Payload.First();
            var tokenValue = x.Payload.Select(kvp => kvp.Value).ToList();
            if (tokenValue[1].ToString() != "Admin") return null;

            var q = ctx.User.AsQueryable().AsNoTracking();
            var result = await q.Select(GuestListDTOExpression).ToListAsync();

            return result;
        }
        public async Task<string> DeleteRoom(RoomDTO dto, string token)
        {
            using var ctx = context();
            Decode(token);
            var x = tokenOne;
            var y = x.Payload.First();
            var tokenValue = x.Payload.Select(kvp => kvp.Value).ToList();
            if (tokenValue[1].ToString() != "Admin") return null;
            if (!dto.Id.HasValue) return "null";

            var bookingList = await ctx.Booking.Where(x => x.RoomId == dto.Id).ToListAsync();
            if (bookingList != null)
            {
                foreach (var booking in bookingList)
                {
                    Payments? payments = await ctx.Payments.FindAsync(booking.Id);
                    if (payments != null) ctx.Payments.Remove(payments);
                    ctx.Booking.Remove(booking);
                    await ctx.SaveChangesAsync();
                }
            }
            Room? room = await ctx.Room.FindAsync(dto.Id.Value);
            if (room == null) return "null";
            ctx.Room.Remove(room);
            await ctx.SaveChangesAsync();
            return "done";
        }
        public async Task<EditRoomDTO?> EditRoom(EditRoomDTO dto)
        {
            using var ctx = context();
            Decode(dto.Token);
            var x = tokenOne;
            var y = x.Payload.First();
            var tokenValue = x.Payload.Select(kvp => kvp.Value).ToList();
            if (tokenValue[1].ToString() != "Admin") return null;
            if (!dto.Id.HasValue) return null;
            Room? room = await ctx.Room.FindAsync(dto.Id);
            if (room == null) return null;
            room.RoomNumber = dto.RoomNumber != 0 ? dto.RoomNumber : room.RoomNumber;
            room.RoomPrice = dto.RoomPrice != 0 ? dto.RoomPrice : room.RoomPrice;
            room.RoomCapacity = dto.RoomCapacity != 0 ? dto.RoomCapacity : room.RoomCapacity;
            //room.RoomPhoto = dto.RoomPhoto != null ? dto.RoomPhoto : room.RoomPhoto;
            room.RoomStatus = dto.RoomStatus != null ? dto.RoomStatus : room.RoomStatus;
            room.RoomType = dto.RoomType != null ? dto.RoomType : room.RoomType;
            await ctx.SaveChangesAsync();
            room.RoomStatus = dto.RoomStatus;
#pragma warning disable CS8603 // Possible null reference return.
            return RoomEditDTOExpression.Invoke(room);
#pragma warning disable CS8603 // Possible null reference return.
        }

        public async Task<AddRoomDTO> AddRoom(AddRoomDTO dto)
        {
            using var ctx = context();
            Decode(dto.Token);
            var x = tokenOne;
            var y = x.Payload.First();
            var tokenValue = x.Payload.Select(kvp => kvp.Value).ToList();
            if (tokenValue[1].ToString() != "Admin") return null;
            Room room = new();
            await ctx.Room.AddAsync(room);
            room.RoomNumber = dto.RoomNumber != 0 ? dto.RoomNumber : room.RoomNumber;
            room.RoomPrice = dto.RoomPrice != 0 ? dto.RoomPrice : room.RoomPrice;
            room.RoomCapacity = dto.RoomCapacity != 0 ? dto.RoomCapacity : room.RoomCapacity;
            //room.RoomPhoto = dto.RoomPhoto != null ? dto.RoomPhoto : room.RoomPhoto;
            room.RoomStatus = dto.RoomStatus != null ? dto.RoomStatus : room.RoomStatus;
            room.RoomType = dto.RoomType != null ? dto.RoomType : room.RoomType;
            await ctx.SaveChangesAsync();
#pragma warning disable CS8603 // Possible null reference return.
            return RoomAddDTOExpression.Invoke(room);
#pragma warning disable CS8603 // Possible null reference return.
        }
        public async Task<String> Payment(int bookingId, string token, decimal amount)
        {
            using var ctx = context();
            Decode(token);
            var x = tokenOne;
            var tokenValue = x.Payload.Select(kvp => kvp.Value).ToList();
            if (tokenValue[1].ToString() != "Admin") return null;
            Booking? bookink = await ctx.Booking.FindAsync(bookingId);
            if (bookink == null) return "null";
            DateTime startDate = (DateTime)bookink.BookingFrom; // Ustawienie pierwszej daty
            DateTime endDate = (DateTime)bookink.BookingTo;   // Ustawienie drugiej daty
            int differenceInDays = (endDate - startDate).Days;
            Payments payments = new();
            await ctx.Payments.AddAsync(payments);
            payments.PaymentType = PaymentType.Cash;
            payments.Paid = (int)bookink.ToPay * differenceInDays;
            payments.BookingId = bookingId;
            await ctx.SaveChangesAsync();

            //bookink. = (Common.Enums.PaymentType)enu;
            //await ctx.SaveChangesAsync();
            //return "done";
            return "done";
        }
        public async Task<List<BookingDTO>> GetBookingList(string token)
        {
            using var ctx = context();
            Decode(token);
            var x = tokenOne;
            var tokenValue = x.Payload.Select(kvp => kvp.Value).ToList();
            if (tokenValue[1].ToString() != "Admin") return null;
            var q = ctx.Booking.AsQueryable().AsNoTracking();
            var result = await q.Select(BookingListDTOExpression).ToListAsync();
            var u = ctx.Room.AsQueryable().AsNoTracking();
            var resultRoom = await u.Select(RoomListDTOExpression).ToListAsync();
            foreach (var r in result)
            {
                DateTime startDate = (DateTime)r.BookingFrom; // Ustawienie pierwszej daty
                DateTime endDate = (DateTime)r.BookingTo;   // Ustawienie drugiej daty
                int differenceInDays = (endDate - startDate).Days;
                Room? room = await ctx.Room.FindAsync(r.RoomId);
                r.payment = (room.RoomPrice * differenceInDays);
                r.RoomNr = room.RoomNumber;
            }
            return result;
        }
        public async Task<BookingDTO> GetBooking(int bookingId, string token)
        {
            using var ctx = context();
            Decode(token);
            var x = tokenOne;
            var tokenValue = x.Payload.Select(kvp => kvp.Value).ToList();
            if (tokenValue[1].ToString() != "Admin") return null;
            Booking? booking = await ctx.Booking.FindAsync(bookingId);
            if (booking == null) return null;
            var result = BookingListDTOExpression.Invoke(booking);
            DateTime startDate = (DateTime)result.BookingFrom; // Ustawienie pierwszej daty
            DateTime endDate = (DateTime)result.BookingTo;   // Ustawienie drugiej daty
            int differenceInDays = (endDate - startDate).Days;
            Room? room = await ctx.Room.FindAsync(result.RoomId);
            result.payment = (room.RoomPrice * differenceInDays);
            result.RoomNr = room.RoomNumber;
            return result;
        }
        public async Task<List<PaymentDTO>> GetPaymentList(string token)
        {
            using var ctx = context();
            Decode(token);
            var x = tokenOne;
            var tokenValue = x.Payload.Select(kvp => kvp.Value).ToList();
            if (tokenValue[1].ToString() != "Admin") return null;
            var q = ctx.Payments.AsQueryable().AsNoTracking();
            var result = await q.Select(PaymentListDTOExpression).ToListAsync();
            return result;
        }
        public async Task<PaymentDTO> GetPayment(int paymentId, string token)
        {
            using var ctx = context();
            Decode(token);
            var x = tokenOne;
            var tokenValue = x.Payload.Select(kvp => kvp.Value).ToList();
            if (tokenValue[1].ToString() != "Admin") return null;
            Payments? payment = await ctx.Payments.FindAsync(paymentId);
            if (payment == null) return null;
            return PaymentListDTOExpression.Invoke(payment);
        }
        public async Task<BookingDTO> BookingRoom(AdminBookingDTO dto)
        {
            using var ctx = context();
            Decode(dto.token);
            var x = tokenOne;
            var tokenValue = x.Payload.Select(kvp => kvp.Value).ToList();
            if (tokenValue[1].ToString() != "Admin") return null;
            if (dto.Name == null) return null;

            Room? room = await ctx.Room.FindAsync(dto.RoomId);
            if (room == null) return null;
            if (room.RoomStatus == RoomStatus.reserved) return null;
            room.RoomStatus = RoomStatus.reserved;
            await ctx.SaveChangesAsync();

            User user = new();
            await ctx.User.AddAsync(user);
            user.Name = dto.Name;
            user.Surname = dto.Surname;
            await ctx.SaveChangesAsync();

            Booking booking = new();
            await ctx.Booking.AddAsync(booking);
            UpdateBooking(booking, dto, user, room);
            await ctx.SaveChangesAsync();

#pragma warning disable CS8603 // Possible null reference return.
            return BookingAddEditDTOExpression.Invoke(booking);
#pragma warning restore CS8603 // Possible null reference return.
        }
        private static void UpdateBooking(Booking booking, AdminBookingDTO dto, User user, Room room)
        {
            TimeSpan roznica = (TimeSpan)(dto.BookingTo - dto.BookingFrom);
            booking.UserId = user.Id;
            booking.RoomId = dto.RoomId ?? booking.RoomId;
            booking.ToPay = (decimal)room.RoomPrice * (decimal)roznica.TotalDays;
            booking.BookingTo = dto.BookingTo ?? booking.BookingTo;
            booking.BookingFrom = dto.BookingFrom ?? booking.BookingFrom;

        }
        private static void UpdateRoom(Room room, RoomDTO dto)
        {

            room.RoomNumber = dto.RoomNumber != 0 ? dto.RoomNumber : room.RoomNumber;
            room.RoomPrice = dto.RoomPrice != 0 ? dto.RoomPrice : room.RoomPrice;
            room.RoomCapacity = dto.RoomCapacity != 0 ? dto.RoomCapacity : room.RoomCapacity;
            room.RoomPhoto = dto.RoomPhoto != null ? dto.RoomPhoto : room.RoomPhoto;
            room.RoomStatus = dto.RoomStatus != null ? dto.RoomStatus : room.RoomStatus;
            room.RoomType = dto.RoomType != null ? dto.RoomType : room.RoomType;

        }
        internal static readonly Expression<Func<Booking, BookingDTO?>> BookingAddEditDTOExpression = x => new BookingDTO()
        {
            //GuestId = x.UserId,
            RoomId = x.RoomId,
            BookingFrom = x.BookingFrom,
            BookingTo = x.BookingTo
        };
        internal static readonly Expression<Func<Payments, PaymentDTO?>> PaymentListDTOExpression = x => new PaymentDTO()
        {
            Id = x.Id,
            BookingId = x.BookingId,
            Paid = x.Paid,
            PaymentType = x.PaymentType
        };
        internal static readonly Expression<Func<Booking, BookingDTO?>> BookingListDTOExpression = x => new BookingDTO()
        {
            Id = x.Id,
            //RoomNr = x.RoomId,//hmmmmmmmmmm
            GuestId = x.UserId,
            RoomId = x.RoomId,
            BookingFrom = x.BookingFrom,
            BookingTo = x.BookingTo
        };
        internal static readonly Expression<Func<Room, RoomDTO?>> RoomAddEditDTOExpression = x => new RoomDTO()
        {
            Id = x.Id,
            RoomNumber = x.RoomNumber,
            RoomPrice = x.RoomPrice,
            RoomCapacity = x.RoomCapacity,
            RoomPhoto = x.RoomPhoto,
            RoomType = x.RoomType.Value,
            RoomStatus = x.RoomStatus.Value,
            //RoomTypeId = x.RoomTypeId
        };
        internal static readonly Expression<Func<Room, EditRoomDTO?>> RoomEditDTOExpression = x => new EditRoomDTO()
        {
            Id = x.Id,
            RoomNumber = x.RoomNumber,
            RoomPrice = x.RoomPrice,
            RoomCapacity = x.RoomCapacity,
            RoomType = x.RoomType.Value,
            RoomStatus = x.RoomStatus.Value,
            //RoomTypeId = x.RoomTypeId
        };
        internal static readonly Expression<Func<Room, AddRoomDTO?>> RoomAddDTOExpression = x => new AddRoomDTO()
        {
    
            RoomNumber = x.RoomNumber,
            RoomPrice = x.RoomPrice,
            RoomCapacity = x.RoomCapacity,          
            RoomType = x.RoomType.Value,
            RoomStatus = x.RoomStatus.Value,
            //RoomTypeId = x.RoomTypeId
        };
        //internal static readonly Expression<Func<RoomType, RoomTypeDTO?>> RoomTypeAddDTOExpression = x => new RoomTypeDTO()
        //{
        //    Id = x.Id,
        //    RoomType = x.Type

        //};
        internal static readonly Expression<Func<User, GuestDTO?>> GuestListDTOExpression = x => new GuestDTO()
        {
            Id = x.Id,
            Email = x.Email,
            //Password = x.PasswordHash,
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
            RoomStatus = x.RoomStatus.Value,
            RoomType = x.RoomType.Value
            //RoomTypeId = x.RoomTypeId
        };
    }
}
