using ImportExportManagement_API.Models;
using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.Repositories;
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
        [HttpGet("/api/inventories")]
        public async Task<ActionResult<IEnumerable<Inventory>>> SearchInventory([FromQuery] PaginationParam paging, [FromQuery] InventoryFilter filter)
        {
            Pagination<Inventory> listInventory = await _repo.GetAllInventory(paging, filter);
            return Ok(listInventory);
        }

        //tạo phiếu nhập kho
        //hàm này chỉ được chạy tự động, khi transaction ở trạng thái success
        [HttpPost]
        public async Task<ActionResult> CreateInventory(Inventory inventory)
        {
            _repo.Insert(inventory);
            await _repo.SaveAsync();
            return Ok(inventory);
        }

        //check ngày này có tồn tại phiếu nhập kho chưa
        [HttpGet("{dateRecord}")]
        public async Task<ActionResult<IdentityCard>> GetIdentityCard(DateTime dateRecord)
        {
            var identityCard = _repo.CheckExistDateRecord(dateRecord);

            if (identityCard == null)
            {
                return NotFound();
            }

            return Ok(identityCard);
        }
    }
}
