using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelProject.Services.DTO
{
    public class AdminBookingDTO
    {
        public string token { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public int? RoomId { get; set; }
        //public int RoomNr { get; set; }
        public DateTime? BookingFrom { get; set; }
        public DateTime? BookingTo { get; set; }
    }
}
