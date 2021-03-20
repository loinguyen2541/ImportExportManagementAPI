using ImportExportManagementAPI.DataStore;
using ImportExportManagementAPI.ModelWeb;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/chart")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly IHubContext<ChartHub> hub;

        public ChartController(IHubContext<ChartHub> hub)
        {
            this.hub = hub;
        }
        [HttpGet]
        public IActionResult Get()
        {
            var timerManager = new TimerManager(()=> hub.Clients.All.SendAsync("transferchartdata",
                DataManager.GetData()));
            return Ok(new { Message = "Request Compelete"});
        }
    }
}
