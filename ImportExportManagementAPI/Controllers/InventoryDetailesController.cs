using ImportExportManagement_API.Models;
using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        public async Task<ActionResult<IdentityCard>> GetInventoryDetails([FromQuery] PaginationParam paging, [FromQuery] InventoryDetailFilter filter)
        {
            Pagination<InventoryDetail> listInventoryDetail = await _repo.GetInventoryDetail(paging, filter);
            return Ok(listInventoryDetail);
        }
        [HttpGet("{Inventoryid}")]
        [AllowAnonymous]
        public async Task<ActionResult<IdentityCard>> GetInventoryDetailByInventory([FromQuery] PaginationParam paging, [FromQuery] InventoryDetailFilter filter)
        {
            Pagination<InventoryDetail> listInventoryDetail = await _repo.GetInventoryDetail(paging, filter);
            return Ok(listInventoryDetail);
        }
        [HttpGet("types")]
        [AllowAnonymous]
        public ActionResult<Object> GetTransType()
        {
            return Ok(Enum.GetValues(typeof(InventoryDetailType)).Cast<InventoryDetailType>().ToList());
        }

        [HttpGet("report")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<InventoryDetail>>> Search([FromQuery] PaginationParam paging, [FromQuery] InventoryFilter filter)
        {
            Pagination<InventoryDetail> listInventory = await _repo.GetReportPartner(paging, filter);
            return Ok(listInventory);
        }

        [HttpGet("reportIventoryDetail")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<InventoryDetail>>> report([FromQuery] ReportFilter filter)
        {
            List<InventoryDetail> listInventory = await _repo.getDataReportInventoryDetail(filter);
            return Ok(listInventory);
        }




    }
}
