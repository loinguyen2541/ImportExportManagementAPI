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
        //get transaction
        [HttpGet]
        public async Task<ActionResult<Pagination<Transaction>>> GetAllTransaction([FromQuery] PaginationParam paging, [FromQuery] TransactionFilter filter)
        {
            Pagination<Transaction> listTransaction = await _repo.GetAllAsync(paging, filter);
            return Ok(listTransaction);
        }

        //get number of lastest transaction
        [HttpGet("/api/transactions/last")]
        public async Task<ActionResult<Pagination<Transaction>>> GetLastTransaction([FromQuery] PaginationParam paging)
        {
            Pagination<Transaction> listTransaction = await _repo.GetLastIndex(paging);
            return Ok(listTransaction);
        }

        //add transaction
        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTransaction(Transaction transaction)
        {
            _repo.Insert(transaction);
            await _repo.SaveAsync();

            return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
        }
        
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
        [HttpGet("partners/search")]
        public async Task<ActionResult<Pagination<Transaction>>> GetTransactionByPartnerId([FromQuery]PaginationParam paging, [FromQuery] int id)
        {
            Pagination<Transaction> trans = await _repo.GetTransByPartnerIdAsync(paging,id);

            if (trans == null)
            {
                return NotFound();
            }

            return trans;
        }
        [HttpGet("types")]
        public ActionResult<Object> GetTransType()
        {
            return Ok(Enum.GetValues(typeof(TransactionType)).Cast<TransactionType>().ToList());
        }
        [HttpGet("states")]
        public ActionResult<Object> GetTransState()
        {
            return Ok(Enum.GetValues(typeof(TransactionStatus)).Cast<TransactionStatus>().ToList());
        }
    }
}
