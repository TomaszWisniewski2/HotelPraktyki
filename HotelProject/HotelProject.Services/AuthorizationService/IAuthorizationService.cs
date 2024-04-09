using HotelProject.DAL.Entities;
using HotelProject.Services.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelProject.Services.AuthorizationService
{
    public interface IAuthorizationService
    {
        Task<ActionResult<User>> Register(RegisterDTO request);
        Task<TokenDTO> Login(RegisterDTO request);
        ActionResult<User> GetMe(string token);
        //void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
    }
}
