using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportExportManagement_API;
using ImportExportManagement_API.Models;
using ImportExportManagementAPI.Repositories;
using ImportExportManagementAPI.Models;

namespace ImportExportManagementAPI
{
    [Route("api/partners")]
    [ApiController]
    public class PartnersController : ControllerBase
    {
        PartnerRepository _repo;
        public PartnersController()
        {
            _repo = new PartnerRepository();
        }

        // GET: api/Partners
        [HttpGet]
        public ActionResult<IEnumerable<Partner>> GetPartners()
        {
            return _repo.GetPartners();
        }

        // GET: api/Partners
        [HttpGet("/api/partners/search")]
        public async Task<ActionResult<IEnumerable<Partner>>> SearchPartnersByFilterAsync([FromQuery] PartnerFilter partnerFilter)
        {
            List<Partner> schedules = await _repo.GetAllAsync(partnerFilter);
            return Ok(schedules);
        }


        // GET: api/Partners/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Partner>> GetPartner(int id)
        {
            var partner = await _repo.GetByIDAsync(id);

            if (partner == null)
            {
                return NotFound();
            }

            return partner;
        }

        // PUT: api/Partners/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPartner(int id, Partner partner)
        {
            if (id != partner.PartnerId)
            {
                return BadRequest();
            }

            _repo.Update(partner);

            try
            {
                await _repo.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_repo.Exist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Partners
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Partner>> PostPartner(Partner partner)
        {
            _repo.Insert(partner);
            await _repo.SaveAsync();

            return CreatedAtAction("GetPartner", new { id = partner.PartnerId }, partner);
        }

        // DELETE: api/Partners/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePartner(int id)
        {
            var partner = await _repo.GetByIDAsync(id);
            if (partner == null)
            {
                return NotFound();
            }

            _repo.DeletePartner(partner);
            await _repo.SaveAsync();

            return NoContent();
        }
    }
}
