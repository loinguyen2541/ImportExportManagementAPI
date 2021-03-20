using ImportExportManagement_API.Models;
using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        AccountRepository _repo;
        public AccountController()
        {
            _repo = new AccountRepository();
        }

        // GET: api/Partners
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<Account>> GetAccounts([FromQuery] AccountStatus status)
        {
            return _repo.GetAccounts(status);
        }


        // GET: api/Partners/5
        [HttpGet("account")]
        [AllowAnonymous]
        public async Task<ActionResult<Account>> GetAccount([FromQuery]string username)
        {
            var account = await _repo.GetAccount(username);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        // PUT: api/Partners/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut()]
        [AllowAnonymous]
        public async Task<IActionResult> PutAccount(string username, Account account)
        {
            if (username != account.Username)
            {
                return BadRequest();
            }

            _repo.Update(account);

            try
            {
                await _repo.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_repo.Exist(username))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(account);
        }

        // POST: api/Partners
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
            _repo.Insert(account);
            await _repo.SaveAsync();

            return CreatedAtAction("GetAccount", new { username = account.Username }, account);
        }

        // DELETE: api/Partners/5
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteAccount(string username)
        {
            var account = await _repo.GetByIDAsync(username);
            if (account == null)
            {
                return NotFound();
            }

            _repo.DeleteAccount(account);
            await _repo.SaveAsync();

            return NoContent();
        }
    }
}
