using HotelProject.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelProject.DAL.Entities
{
    public class Payments : Entity
    {
        public decimal Paid { get; set; }
        public PaymentType PaymentType { get; set; }

        [ForeignKey(nameof(Booking))]
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }
    }
}
