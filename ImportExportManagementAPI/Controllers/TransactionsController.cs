using ImportExportManagement_API.Models;
using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionRepository _repo;
        public TransactionsController()
        {
            _repo = new TransactionRepository();
        }
        //KhanhBDB
        //get transaction
        [HttpGet]
        public async Task<ActionResult<List<Transaction>>> GetAllTransaction([FromQuery] TransactionFilter filter)
        {
            List<Transaction> listTransaction = await _repo.GetAllAsync(filter);
            return Ok(listTransaction);
        }
        //KhanhBDB
        //add transaction
        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTransaction(Transaction transaction)
        {
            _repo.Insert(transaction);
            await _repo.SaveAsync();

            return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
        }
        //KhanhBDB
        //update transaction information
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, Transaction trans)
        {
            if (id != trans.TransactionId)
            {
                return BadRequest();
            }

            _repo.Update(trans);

            try
            {
                await _repo.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_repo.GetByID(id) == null)
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

        //KhanhBDB
        //get transaction by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            Transaction trans = await _repo.GetByIDIncludePartnerAsync(id);

            if (trans == null)
            {
                return NotFound();
            }

            return trans;
        }
    }
}
