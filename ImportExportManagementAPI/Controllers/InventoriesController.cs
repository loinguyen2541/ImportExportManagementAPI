using ImportExportManagement_API.Models;
using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/inventorydetails")]
    [ApiController]
    public class InventoryDetailsController : ControllerBase
    {
        InventoryRepository _repo;
        public InventoryDetailsController()
        {
            _repo = new InventoryRepository();
        }

        // GET: api/inventories
        [HttpGet("/api/inventories/search")]
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
            bool insert = _repo.InsertInventory(inventory);
            if (insert)
            {
                await _repo.SaveAsync();
                return Ok();
            }
            return BadRequest("Duplicates date record");
        }
    }
}
