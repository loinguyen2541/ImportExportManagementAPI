using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/systemconfigs")]
    [ApiController]
    public class SystemConfigController : ControllerBase
    {
        private readonly SystemConfigRepository _systemConfigRepository;

        public SystemConfigController(SystemConfigRepository systemConfigRepository)
        {
            _systemConfigRepository = systemConfigRepository;
        }

        [HttpGet("storge-capacity")]
        public ActionResult<String> GetStorgeCapacity()
        {
            return Ok(_systemConfigRepository.GetStorageCapacity());
        }
    }
}
