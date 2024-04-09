using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelProject.Services.DTO
{
    public class BookingDTO
    {
         public int? Id { get; set; }
        public int? GuestId { get; set; }
        public int? RoomId { get; set; }
        public int RoomNr { get; set; }
        public float payment { get; set; }
        public DateTime? BookingFrom { get; set; }
        public DateTime? BookingTo { get; set; }


    }
}
