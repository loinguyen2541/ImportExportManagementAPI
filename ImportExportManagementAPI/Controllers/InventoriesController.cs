using ImportExportManagement_API.Models;
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
        InventoryRepository _repo;
        public InventoriesController()
        {
            _repo = new InventoryRepository();
        }

        // GET: api/inventories
        [HttpGet]
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
        [HttpGet("totalByMonth")]
        [AllowAnonymous]
        public ActionResult<Object> GetTotalByDateFromDateToTypeFloat(DateTime dateFrom, DateTime dateTo)
        {
            return Ok(_repo.TotalWeightInventoryFloatByMonth(dateFrom, dateTo));
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


        //lấy tổng khối lượng nhập/xuất theo ngày
        [HttpGet("reportTransaction")]
        public ActionResult<Inventory> ReportTransaction(DateTime currentDate, int partnerID)
        {
            Inventory total = _repo.ReportTransaction(currentDate, partnerID);
            return Ok(total);
        }
    }
}
