using ImportExportManagement_API.Models;
using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        [HttpPost]
        public async Task<ActionResult> CreateInventoryDetail(DateTime dateRecord, Transaction trans)
        {
             _repo.InsertInventoryDetailAsync(dateRecord, trans);
            await _repo.SaveAsync();
            return Ok();
        }
    }
}
