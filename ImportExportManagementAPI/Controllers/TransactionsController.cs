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
        //KhanhBDB
        //add transaction
        [HttpPost("manual")]
        public async Task<ActionResult> CreateTransactionByManual(Transaction transaction)
        {
            bool check = await _repo.CreateTransactionAsync(transaction, "manual");
            if (!check)
            {
                return BadRequest("Invalid input");
            }
            await _repo.SaveAsync();
            return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
        }
        //add transaction
        [HttpPost("automatic")]
        public async Task<ActionResult<Transaction>> CreateTransactionByAutomatic(String cardId, float weightIn)
        {
            IdentityCardRepository cardRepo = new IdentityCardRepository();
            IdentityCard checkCard = await cardRepo.checkCardAsync(cardId);
            if (checkCard != null)
            {
                DateTime timeIn = DateTime.Now;
                Transaction transaction = new Transaction() { IdentityCardId = cardId, TimeIn = timeIn, WeightIn = weightIn, TransactionStatus = TransactionStatus.Progessing, PartnerId = checkCard.PartnerId, GoodsId = 1 };
                await _repo.CreateTransactionAsync(transaction, "automatic");
                await _repo.SaveAsync();

                return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
            }
            return BadRequest("Card is not exised");
        }
        //KhanhBDB
        //update transaction information => manual
        [HttpPut("manual/{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, Transaction trans)
        {
            String checkUpdate = await _repo.UpdateTransaction(trans, id);
            if (checkUpdate.Length == 0)
            {
                return NoContent();
            }
            return BadRequest(checkUpdate);
        }

        /*
         Tìm giá trị id của transaction mới nhất của thẻ đó:
        + nếu trans ở trạng thái success => bỏ qua
        + nếu trans ở trạng thái processing => trả về giá trị id trans để update weight lần 2
         */

        [HttpPut("automatic/{cardId}")]
        public async Task<ActionResult<Transaction>> UpdateTransactionByAutomatic(String cardId, float weightOut)
        {
            DateTime timeOut = DateTime.Now;
            bool check = await _repo.UpdateTransScandCardAsync(cardId, weightOut, timeOut);
            if (check)
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
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
        public async Task<ActionResult<Pagination<Transaction>>> GetTransactionByPartnerId([FromQuery] PaginationParam paging, [FromQuery] int id)
        {
            Pagination<Transaction> trans = await _repo.GetTransByPartnerIdAsync(paging, id);

            if (trans == null)
            {
                return BadRequest();
            }
            return Ok(trans);
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
