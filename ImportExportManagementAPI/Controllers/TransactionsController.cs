using ImportExportManagement_API.Models;
using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<List<Transaction>>> GetTransaction([FromQuery] Paging paging, [FromQuery] TransactionFilter filter)
        {
            List<Transaction> listTransaction = await _repo.GetAllAsync(paging, filter);
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
        [HttpPut]
        public async Task<ActionResult<Transaction>> UpdateTransaction(Transaction transaction)
        {
            _repo.Update(transaction);
            await _repo.SaveAsync();

            return Ok();
        }
    }
}
