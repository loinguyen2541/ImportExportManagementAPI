﻿using ImportExportManagement_API.Models;
using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.Objects;
using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/inventories")]
    [ApiController]
    public class InventoriesController : ControllerBase
    {
        private readonly InventoryRepository _repo;
        private readonly ScheduleRepository _repoSchedule;
        private readonly TransactionRepository transactionRepository;
        public InventoriesController()
        {
            _repoSchedule = new ScheduleRepository();
            _repo = new InventoryRepository();
            transactionRepository = new TransactionRepository();
        }

        // GET: api/inventories
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetAll()
        {
            List<Inventory> listInventory = await _repo.GetAllAsync();
            return Ok(listInventory);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Inventory>>> SearchInventory([FromQuery] PaginationParam paging, [FromQuery] InventoryFilter filter)
        {
            Pagination<Inventory> listInventory = await _repo.GetAllInventory(paging, filter);
            return Ok(listInventory);
        }

        //check ngày này có tồn tại phiếu nhập kho chưa
        [HttpGet("{dateRecord}")]
        [AllowAnonymous]
        public async Task<ActionResult<IdentityCard>> GetDateRecord(DateTime dateRecord)
        {
            var identityCard = await _repo.CheckExistDateRecord(dateRecord);

            if (identityCard == null)
            {
                return NotFound();
            }

            return Ok(identityCard);
        }
        //tạo phiếu nhập kho
        //hàm này chỉ được chạy tự động, khi transaction ở trạng thái success
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CreateInventory(Inventory inventory)
        {
            _repo.Insert(inventory);
            await _repo.SaveAsync();
            return Ok(inventory);
        }

        //lấy tổng khối lượng nhập/xuất theo ngày
        [HttpGet("total")]
        [AllowAnonymous]
        public async Task<ActionResult<String>> GetTotalByDateType(DateTime date, String type)
        {
            InventoryDetailType inventoryDetailType;
            if (Enum.TryParse(type, out inventoryDetailType))
            {
                String total = await _repo.TotalWeightInventory(date, inventoryDetailType);
                return Ok(total);
            }
            return BadRequest();
        }

        [HttpGet("totalFloat")]
        [AllowAnonymous]
        public ActionResult<ObjectTotalImportExportToday> GetTotalByDateTypeFloat(DateTime date)
        {
            return Ok(_repo.TotalWeightInventoryFloat(date).Result);
        }
        [HttpGet("totalByPartner")]
        [AllowAnonymous]
        public ActionResult<ObjectTotalImportExportToday> GetTotalByPartner(string searchDate,int partnerId)
        {
           
            float totalWeightTransactionByPartner = _repo.TotalWeightInventoryByPartner(searchDate, partnerId);
            float totalWeightScheduleByPartner = _repoSchedule.GetTotalSchedule(partnerId, searchDate).Result;
            int transactionCount = transactionRepository.CountTransactionByPartner(searchDate, partnerId);
            int scheduleCount = _repoSchedule.CountScheduleByPartner(searchDate, partnerId);

            return Ok( new
            {
                totalWeightTransactionByPartner = totalWeightTransactionByPartner,
                totalWeightScheduleByPartner= totalWeightScheduleByPartner,
                transactionCount = transactionCount,
                scheduleCount = scheduleCount,
            });
        }
        [HttpGet("totalByMonth")]
        [AllowAnonymous]
        public ActionResult<List<TotalInventoryDetailedByDate>> GetTotalByDateFromDateToTypeFloat(DateTime dateFrom, DateTime dateTo)
        {
            return Ok(_repo.TotalWeightInventoryFloatByMonth(dateFrom, dateTo));
        }

        [HttpGet("totalByMonthForPartner")]
        [AllowAnonymous]
        public ActionResult<List<TotalInventoryDetailedByDate>> GetTotalByDateFromDateToByPartnerId(DateTime dateFrom, DateTime dateTo,int partnerId)
        {
            return Ok(_repo.TotalWeightInventoryFloatByMonthForPartner(dateFrom, dateTo, partnerId));
        }

        //lấy tổng khối lượng nhập/xuất theo ngày
        [HttpGet("reportPartner")]
        [AllowAnonymous]
        public ActionResult<Inventory> reportPartner(DateTime DateFrom, DateTime DateTo, string partnerName)
        {
            List<Inventory> total = _repo.ReportPartner(DateFrom, DateTo, partnerName);
            return Ok(total);
        }
        [HttpGet("reportInventory")]
        [AllowAnonymous]
        public ActionResult<Inventory> reportInventory(DateTime DateFrom, DateTime DateTo)
        {
            List<Inventory> total = _repo.ReoportInventory(DateFrom, DateTo);
            return Ok(total);
        }

        //lấy tổng khối lượng nhập/xuất theo ngày của partner
        [HttpGet("partner-totalweight")]
        public async Task<ActionResult<float>> TotalWeightByDateOfPartner(DateTime fromDate, DateTime toDate, int partnerId)
        {
            float total = 0;
            if (fromDate.Equals(toDate))
            {
                total = await _repo.TotalWeightInventoryOfPartnerByDate(fromDate, partnerId);
            }
            else
            {
                total = await _repo.TotalWeightInventoryOfPartnerByTime(fromDate, toDate, partnerId);
            }
            return Ok(total);
        }

        //lấy import/export theo khaorng thời gian của partner
        [HttpGet("partner-import-export")]
        public async Task<ActionResult<List<InventoryDetail>>> GetImportExportByTime(DateTime fromDate, DateTime toDate, int partnerId)
        {
            List<InventoryDetail> listDetail = await _repo.GetImportExportByTime(fromDate, toDate, partnerId);
            return Ok(listDetail);
        }
    }
}
