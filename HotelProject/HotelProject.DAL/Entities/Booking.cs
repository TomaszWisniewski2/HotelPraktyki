using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelProject.Common.Enums;
namespace HotelProject.DAL.Entities
{
    public class Booking : Entity
    {
        public DateTime BookingFrom { get; set; }
        public DateTime BookingTo { get; set; }
        public decimal ToPay { get; set; }//Do zapłaty
        

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User? User { get; set; }

        [ForeignKey(nameof(Room))]
        public int RoomId { get; set; }
        public Room? Room { get; set; }

        public ICollection<Payments>? Payments { get; set; }
    }
}
