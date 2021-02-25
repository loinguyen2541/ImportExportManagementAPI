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
        //get transaction by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var trans = await _repo.GetByIDAsync(id);

            if (trans == null)
            {
                return NotFound();
            }

            return trans;
        }

        //KhanhBDB
        //add transaction
        [HttpPost("manual")]
        public async Task<ActionResult<Transaction>> CreateTransactionByManual(Transaction transaction)
        {
            _repo.CreateTransaction(transaction);
            await _repo.SaveAsync();

            return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
        }
        //add transaction
        [HttpPost("automatic")]
        public async Task<ActionResult<Transaction>> CreateTransactionByAutomatic(String cardId, float weightIn, DateTime timeIn)
        {
            Transaction transaction = new Transaction() { IdentityCardId = cardId, TimeIn = timeIn, WeightIn = weightIn, TransactionStatus = TransactionStatus.Progessing };
            _repo.CreateTransaction(transaction);
            await _repo.SaveAsync();

            return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
        }
        //KhanhBDB
        //update transaction information => manual
        [HttpPut("manual/{id}")]
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

        /*
         Tìm giá trị id của transaction mới nhất của thẻ đó:
        + nếu trans ở trạng thái success => bỏ qua
        + nếu trans ở trạng thái processing => trả về giá trị id trans để update weight lần 2
         */

        [HttpPut("automatic/{cardId}")]
        public async Task<ActionResult<Transaction>> UpdateTransactionByAutomatic(String cardId, float weightOut, DateTime timeOut)
        {
            bool check = _repo.UpdateTransScandCard(cardId, weightOut, timeOut);
            if (check)
            {
                await _repo.SaveAsync();
            }
            else
            {
                return BadRequest();
            }
            return NoContent();
        }
    }
}
