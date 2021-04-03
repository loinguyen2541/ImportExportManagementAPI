﻿using ImportExportManagement_API.Models;
using ImportExportManagementAPI.Hubs;
using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.ModelWeb;
using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        private readonly GoodsRepository _goodsRepository;
        private readonly ScheduleRepository _scheduleRepository;
        private readonly IHubContext<ChartHub> chartHub;
        private readonly IHubContext<TransactionHub> _transactionHub;

        public TransactionsController(IHubContext<ChartHub> chartHub, IHubContext<TransactionHub> transactionHub)
        {
            this.chartHub = chartHub;
            _repo = new TransactionRepository();
            _goodsRepository = new GoodsRepository();
            _transactionHub = transactionHub;
            _scheduleRepository = new ScheduleRepository();
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

        //get number of lastest transaction
        [HttpGet("partners/last")]
        [AllowAnonymous]
        public async Task<ActionResult<Pagination<Transaction>>> GetLastesOfPartner([FromQuery] PaginationParam paging, int partnerId)
        {
            Pagination<Transaction> listTransaction = await _repo.GetLastesOfPartner(paging, partnerId);
            return Ok(listTransaction);
        }

        //KhanhBDB
        //add transaction
        //[HttpPost("manual")]
        //[AllowAnonymous]
        //public async Task<ActionResult> CreateTransactionByManual(Transaction transaction)
        //{
        //    var check = await _repo.CreateTransaction(transaction, "manual");
        //    if (check == null)
        //    {
        //        return BadRequest("Invalid input");
        //    }
        //    await _repo.SaveAsync();
        //    return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
        //}

        //add transaction by android card
        //[HttpPost("androidcard")]
        //[AllowAnonymous]
        //public async Task<ActionResult> CreateTransactionByAndroidCard(Transaction transaction)
        //{
        //    var check = await _repo.CreateTransaction(transaction);
        //    if (check == null)
        //    {
        //        return BadRequest("Invalid input");
        //    }
        //    await _repo.SaveAsync();
        //    return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
        //}

        //add transaction
        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTransactionByNFCCard(String cardId, float weightIn, int partnerId)
        {
            Transaction trans = new Transaction { CreatedDate = DateTime.Now, IdentityCardId = cardId, WeightIn = weightIn, TimeIn = DateTime.Now, TimeOut = DateTime.Now, TransactionStatus = TransactionStatus.Progessing, PartnerId = partnerId };
            Transaction check = await _repo.CreateTransaction(trans);
            if (check != null)
            {
                await _repo.SaveAsync();
                await _transactionHub.Clients.All.SendAsync("TransactionCreated", "reload");
                return CreatedAtAction("GetTransaction", new { id = check.TransactionId }, check);
            }
            return BadRequest("Card is not exist");
        }
        //KhanhBDB
        //update transaction information => manual
        [HttpPut("manual/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateTransaction(int id, Transaction trans)
        {
            Transaction transactionUpdated = await _repo.UpdateTransactionByManual(trans, id);
            if (transactionUpdated != null)
            {
                _goodsRepository.UpdateQuantityOfGood(transactionUpdated.GoodsId, transactionUpdated.WeightIn - transactionUpdated.WeightOut, transactionUpdated.TransactionType);

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
        [HttpPut]
        [AllowAnonymous]
        public async Task<ActionResult<Transaction>> UpdateTransactionByAutomatic(String cardId,int partnerId, float weightOut)
        {
            Transaction transaction = await _repo.UpdateTransactionArduino(cardId, weightOut, partnerId);
            if (transaction != null)
            {
                Task charHub = chartHub.Clients.All.SendAsync("TransactionSuccess", cardId);
                Task transactionhub = _transactionHub.Clients.All.SendAsync("TransactionSuccess", "reload");
                Task<String> updateMiscellaneous = _repo.UpdateMiscellaneousAsync(transaction);
                return Ok();
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

        [HttpGet("partners/searchdate")]
        public async Task<List<Transaction>> GetTransactionOfPartnerByDate(int partnerId, DateTime searchDate)
        {
            List<Transaction> listTrans = await _repo.GetTransOfPartnerByDate(partnerId, searchDate);
            return listTrans;
        }
    }
}
