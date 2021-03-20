﻿using System;
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
using Microsoft.AspNetCore.Authorization;

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/cards")]
    [ApiController]
    public class IdentityCardsController : ControllerBase
    {
        private readonly IdentityCardRepository _repo;

        public IdentityCardsController(IEDbContext context)
        {
            _repo = new IdentityCardRepository();
        }

        // GET: api/IdentityCards
        [HttpGet]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<IEnumerable<IdentityCard>>> GetIdentityCard()
        {
            return await _repo.GetAllAsync();
        }


        // GET: api/Partners
        [HttpGet("searchCard")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<Pagination<IdentityCard>>> SearchCardByFilterAsync([FromQuery] IdentityCardFilter partnerFilter, [FromQuery] PaginationParam paging)
        {
            Pagination<IdentityCard> identityCards = await _repo.GetAllAsync(paging, partnerFilter);
            return identityCards;
        }

        // GET: api/IdentityCards/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<IdentityCard>> GetIdentityCard(String id)
        {
            var identityCard = await _repo.GetByIDAsync(id);

            if (identityCard == null)
            {
                return NotFound();
            }

            return identityCard;
        }

        // PUT: api/IdentityCards/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> PutIdentityCard(String id, IdentityCard identityCard)
        {
            if (!id.Equals(identityCard.IdentityCardId))
            {
                return BadRequest();
            }

            _repo.Update(identityCard);

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

            return Ok();
        }

        // POST: api/IdentityCards
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<IdentityCard>> PostIdentityCard(IdentityCard identityCard)
        {
            _repo.Insert(identityCard);
            await _repo.SaveAsync();

            return CreatedAtAction("GetIdentityCard", new { id = identityCard.IdentityCardId }, identityCard);
        }

        // DELETE: api/IdentityCards/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> DeleteIdentityCard(String id)
        {
            var identityCard = await _repo.GetByIDAsync(id);
            if (identityCard == null)
            {
                return NotFound();
            }

            _repo.DeleteIdentityCard(identityCard);
            await _repo.SaveAsync();

            return NoContent();
        }

        [HttpGet("status")]
        [Authorize(Roles = "Staff")]
        public ActionResult<Object> GetCardStatus()
        {
            return Ok(_repo.GetCardsStatus());
        }
    }
}
