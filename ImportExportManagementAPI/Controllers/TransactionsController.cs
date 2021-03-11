using ImportExportManagement_API.Models;
using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        public async Task<ActionResult<Pagination<Transaction>>> GetAllTransaction([FromQuery] PaginationParam paging, [FromQuery] TransactionFilter filter)
        {
            Pagination<Transaction> listTransaction = await _repo.GetAllAsync(paging, filter);
            return Ok(listTransaction);
        }
        //get transaction
        [HttpGet("inventorydetail")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Transaction>>> GetTransactionByInventoryDetail(int inventoryDetailId)
        {
            List<Transaction> listTransaction = await _repo.GetTransactionByInventoryDetail(inventoryDetailId);
            return Ok(listTransaction);
        }

        //get number of lastest transaction
        [HttpGet("last")]
        [AllowAnonymous]
        public async Task<ActionResult<Pagination<Transaction>>> GetLastTransaction([FromQuery] PaginationParam paging)
        {
            Pagination<Transaction> listTransaction = await _repo.GetLastIndex(paging);
            return Ok(listTransaction);
        }
        //KhanhBDB
        //add transaction
        [HttpPost("manual")]
        [AllowAnonymous]
        public async Task<ActionResult> CreateTransactionByManual(Transaction transaction)
        {
            bool check = true;
            //check = await _repo.CreateTransactionAsync(transaction, "manual");
            if (!check)
            {
                return BadRequest("Invalid input");
            }
            await _repo.SaveAsync();
            return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
        }
        //add transaction
        [HttpPost("automatic")]
        [AllowAnonymous]
        public async Task<ActionResult<Transaction>> CreateTransactionByAutomatic(String cardId, float weightIn)
        {
            Transaction trans = new Transaction { CreatedDate = DateTime.Now, IdentityCardId = cardId, WeightIn = weightIn, TimeIn = DateTime.Now, TransactionStatus = TransactionStatus.Progessing};
            Transaction check = await _repo.CreateTransaction(trans, "Insert");
            if (check != null)
            {
                await _repo.SaveAsync();
                return CreatedAtAction("GetTransaction", new { id = check.TransactionId }, check);
            }
            return BadRequest("Card is not exised");
        }
        //KhanhBDB
        //update transaction information => manual
        [HttpPut("manual/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateTransaction(int id, Transaction trans)
        {
            bool checkUpdate = await _repo.UpdateTransactionByManual(trans, id);
            if (checkUpdate)
            {
                return NoContent();
            }
            return BadRequest();
        }

        /*
         Tìm giá trị id của transaction mới nhất của thẻ đó:
        + nếu trans ở trạng thái success => bỏ qua
        + nếu trans ở trạng thái processing => trả về giá trị id trans để update weight lần 2
         */

        //update
        [HttpPut("automatic/{cardId}")]
        [AllowAnonymous]
        public async Task<ActionResult<Transaction>> UpdateTransactionByAutomatic(String cardId, float weightOut)
        {
            bool check = await _repo.UpdateTransactionArduino(cardId, weightOut, "UpdateArduino");
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
        public ActionResult<Object> GetTransType()
        {
            return Ok(Enum.GetValues(typeof(TransactionType)).Cast<TransactionType>().ToList());
        }
        [HttpGet("states")]
        [AllowAnonymous]
        public ActionResult<Object> GetTransState()
        {
            return Ok(Enum.GetValues(typeof(TransactionStatus)).Cast<TransactionStatus>().ToList());
        }
    }
}
