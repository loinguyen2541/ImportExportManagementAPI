using ImportExportManagement_API.Models;
using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : Controller
    {
        private RoleRepository _repo;
        public RoleController()
        {
            _repo = new RoleRepository();
        }

        [HttpGet("{roleId}")]
        [AllowAnonymous]
        public async Task<ActionResult<Role>> GetPartners(int roleId)
        {
            Role role = await _repo.GetByIDAsync(roleId);
            if (role == null) return BadRequest();
            return Ok(role);
        }

        //[HttpGet("{roleName}")]
        //[AllowAnonymous]
        //public ActionResult<Role> GetPartners(String roleName)
        //{
        //    Role role = _repo.GetRoleByName(roleName);
        //    if (role == null) return BadRequest();
        //    return Ok(role);
        //}
    }
}
