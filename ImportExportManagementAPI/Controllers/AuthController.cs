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
using Microsoft.AspNetCore.Authorization;
using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.Helper;
using Microsoft.Extensions.Options;

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/authentications")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly IConfiguration _config; //to get ke, isssuer from appsetting
        AccountRepository _repo;
        public AuthController(IConfiguration config, IOptions<AppSettings> appSettings)
        {
            _config = config;
            _appSettings = appSettings.Value;
            _repo = new AccountRepository();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<String>> Login([FromBody] AuthenticateModel model)
        {
            var user = await _repo.Login(model, _appSettings);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user.Token);
        }
    }
}
