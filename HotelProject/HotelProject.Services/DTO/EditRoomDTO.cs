using HotelProject.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelProject.Services.DTO
{
    public class EditRoomDTO
    {
        public int? Id { get; set; }
        public string Token { get; set; }
        public int RoomNumber { get; set; }
        public float RoomPrice { get; set; }
        public int RoomCapacity { get; set; }
  
        public RoomType RoomType { get; set; }
        public RoomStatus RoomStatus { get; set; }
    }
}
