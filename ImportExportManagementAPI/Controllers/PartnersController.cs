using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ImportExportManagement_API;
using ImportExportManagement_API.Models;
using ImportExportManagement_API.Repositories;

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/partners")]
    [ApiController]
    public class PartnersController : Controller
    {
        private readonly PartnerRepository _repo;
        public PartnersController()
        {
            _repo = new PartnerRepository();
        }

        //get all partner has status active
        //auto load for transaction create in destop application
        [HttpGet]
        public async Task<ActionResult<List<Partner>>> GetParterActive()
        {
            List<Partner> listPartner = await _repo.GetAllAsync();
            return Ok(listPartner);
        }
    }
}
