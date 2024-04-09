using System.Security.Cryptography;

namespace HotelProject.DAL
{
    public abstract class Entity : IId
    {
        public int Id { get; set; }
    }
}