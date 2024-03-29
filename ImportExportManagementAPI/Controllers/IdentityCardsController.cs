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

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/card")]
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
        public async Task<ActionResult<IEnumerable<IdentityCard>>> GetIdentityCard()
        {
            return await _repo.GetAllAsync();
        }

        // GET: api/IdentityCards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IdentityCard>> GetIdentityCard(int id)
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

            return NoContent();
        }

        // POST: api/IdentityCards
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<IdentityCard>> PostIdentityCard(IdentityCard identityCard)
        {
            _repo.Insert(identityCard);
            await _repo.SaveAsync();

            return CreatedAtAction("GetIdentityCard", new { id = identityCard.IdentityCardId }, identityCard);
        }

        // DELETE: api/IdentityCards/5
        [HttpDelete("{id}")]
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

    }
}
