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
        public async Task<ActionResult<IdentityCard>> GetInventoryDetail([FromQuery] PaginationParam paging, [FromQuery] InventoryDetailFilter filter)
        {
            Pagination<InventoryDetail> listInventoryDetail = await _repo.GetInventoryDetail(paging, filter);
            return Ok(listInventoryDetail);
        }
        [HttpGet("types")]
        public ActionResult<Object> GetTransType()
        {
            return Ok(Enum.GetValues(typeof(InventoryDetailType)).Cast<InventoryDetailType>().ToList());
        }
    }
}
