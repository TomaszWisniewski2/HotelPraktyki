using HotelProject.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelProject.DAL.Entities
{
    public class Room : Entity
    {
        public int RoomNumber { get; set; }
        public float RoomPrice { get; set; }
        public int RoomCapacity { get; set; }
        public string? RoomPhoto { get; set; }
        public RoomStatus? RoomStatus { get; set; } 
        public RoomType? RoomType { get; set; }

        public ICollection<Booking>? Booking { get; set; }
    }
}
