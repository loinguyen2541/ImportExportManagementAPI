using ImportExportManagement_API.Models;
using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/inventorydetails")]
    [ApiController]
    public class InventoryDetailsController : ControllerBase
    {
        InventoryDetailRepository _repo;
        public InventoryDetailsController()
        {
            _repo = new InventoryDetailRepository();
        }
        [HttpGet]
        public async Task<ActionResult<IdentityCard>> GetInventoryDetails([FromQuery] PaginationParam paging, [FromQuery] InventoryDetailFilter filter)
        {
            Pagination<InventoryDetail> listInventoryDetail = await _repo.GetInventoryDetail(paging, filter);
            return Ok(listInventoryDetail);
        }
        [HttpGet("{Inventoryid}")]
        public async Task<ActionResult<IdentityCard>> GetInventoryDetailByInventory([FromQuery] PaginationParam paging, [FromQuery] InventoryDetailFilter filter)
        {
            Pagination<InventoryDetail> listInventoryDetail = await _repo.GetInventoryDetail(paging, filter);
            return Ok(listInventoryDetail);
        }
        [HttpGet("types")]
        public ActionResult<Object> GetTransType()
        {
            return Ok(Enum.GetValues(typeof(InventoryDetailType)).Cast<InventoryDetailType>().ToList());
        }

        [HttpGet("report")]
        public async Task<ActionResult<IEnumerable<InventoryDetail>>> Search([FromQuery] PaginationParam paging, [FromQuery] InventoryFilter filter)
        {
            Pagination<InventoryDetail> listInventory = await _repo.GetDataPartner(paging, filter);
            return Ok(listInventory);
        }


    }
}
