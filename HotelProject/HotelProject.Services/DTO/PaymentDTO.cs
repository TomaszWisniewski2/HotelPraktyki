using HotelProject.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelProject.Services.DTO
{
    public class PaymentDTO
    {
        public int Id {  get; set; }
        public int BookingId { get; set; }
        public decimal Paid { get; set; }
        public PaymentType PaymentType { get; set; }
    }
}
