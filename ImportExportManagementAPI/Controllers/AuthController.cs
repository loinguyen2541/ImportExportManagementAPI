using ImportExportManagement_API.Models;
using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/authentications")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config; //to get ke, isssuer from appsetting
        AccountRepository _repo;
        public AuthController(IConfiguration config)
        {
            _config = config;
            _repo = new AccountRepository();
        }

        [HttpPost("token")]
        public async Task<ActionResult> Login(Account accountToLogin)
        {
            //check username, password
            Account newAccount = await _repo.Login(accountToLogin);
            if (newAccount.Username == "admin" && newAccount.Password == "admin")
            {
                //save user infor into session
                
                //generate token and return it
                var tokenHandler = new JwtSecurityTokenHandler();
                //add claims
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, newAccount.Username));
                claims.Add(new Claim(ClaimTypes.Email, newAccount.Password));
                claims.Add(new Claim(ClaimTypes.Role, newAccount.RoleId.ToString()));

                //create credential
                var secretKey = Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]);
                var symmectricSecurityKey = new SymmetricSecurityKey(secretKey);
                var credentials = new SigningCredentials(symmectricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

                //create token by handler
                var access_token = new JwtSecurityToken(
                    issuer: _config["JwtSettings:Issuer"],
                    audience: _config["JwtSettings:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: credentials
                    );
                return Ok(tokenHandler.WriteToken(access_token));
            }
            return BadRequest("Invalid login");
        }
    }
}
