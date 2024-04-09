using Azure.Core;
using Azure;
using HotelProject.DAL.Db_Access;
using HotelProject.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.AspNetCore.Authorization;
using HotelProject.Services.DTO;
using HotelProject.Common.Enums;

namespace HotelProject.Services.AuthorizationService
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly Func<DatabaseContext> context;
        private readonly IConfiguration _configuration;
        //public static User user = new User();

        public AuthorizationService(Func<DatabaseContext> context, IConfiguration configuration)
        {
            this.context = context;
            _configuration = configuration;
        }

       
        public ActionResult<User> GetMe(string token)
        {

            var stream = token;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;

            
            using var ctx = context();
            //var q = ctx.User.Where(x => x.Email == tokenS.Payload.Acr).;
            var y = tokenS.Payload.Values.First();
            if (y != null) { }
            //var v = y[1];
            var userName = ctx.User.First();
            return userName;
        }
        public async Task<ActionResult<User>> Register(RegisterDTO request)
        {
            using var ctx = context();
            //sprawdzenie czy email istnieje
            User u = ctx.User.FirstOrDefault(x => x.Email == request.Email);
            if (u!=null) return null;
            
            User user = new();
            await ctx.AddAsync(user);

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Email = request.Email;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.UserRole = 0;
            await ctx.SaveChangesAsync();

            return user;
        }


        public async Task<TokenDTO> Login(RegisterDTO request)
        {

                using var ctx = context();
                var user = ctx.User.Where(x => x.Email == request.Email).First();
            TokenDTO tokenDTO = new TokenDTO();
                if (user == null)
                {
                    return null;
                }

                if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                {
                    return null;
                }
                string role = "Guest";
                if (user.UserRole == UserRole.Admin) { role = "Admin"; }
                
                List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role,  role),
                new Claim(ClaimTypes.Sid, user.Id.ToString())

            };

                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                    _configuration.GetSection("AppSettings:Token").Value));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds);

            tokenDTO.Token = new JwtSecurityTokenHandler().WriteToken(token);
                return tokenDTO;
        }




        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
