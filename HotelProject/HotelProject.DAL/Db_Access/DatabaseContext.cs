using HotelProject.Common.Enums;
using HotelProject.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HotelProject.DAL.Db_Access
{
    public class DatabaseContext : DbContext
    {
        private readonly UserManager<User> _userManager;
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    // Dodajemy seeder dla użytkownika admina
            
        //    var adminUser = new User
        //    {
        //        //Id = adminId,
        //        Name = "Admin",
        //        Surname = "Admin",
        //        Email = "Admin@example.com",
        //        UserRole = UserRole.Admin // Załóżmy, że rola admina jest zdefiniowana jako Admin w enumie UserRole
        //    };

        //    // Tworzymy hasło dla admina
        //    var passwordHasher = new PasswordHasher<User>();
        //    var password = "Admin";
        //    using (var hmac = new HMACSHA512())
        //    {
        //       var passwordSalt1 = hmac.Key;
        //       var passwordHash1 = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        //        adminUser.PasswordHash = passwordHash1;
        //    }
            

        //    modelBuilder.Entity<User>().HasData(adminUser);
        //}
        
        public DbSet<User> User { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<Options> Options { get; set; }

    }
}
