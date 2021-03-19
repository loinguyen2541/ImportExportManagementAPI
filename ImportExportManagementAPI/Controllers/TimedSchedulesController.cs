using ImportExportManagementAPI.Hubs;
using ImportExportManagementAPI.Workers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimedSchedulesController : ControllerBase
    {
        TimedGenerateScheduleService TimedControlService;
        private readonly IHubContext<TransactionHub> _transHub;

        public TimedSchedulesController(TimedGenerateScheduleService timedGenerateScheduleService, Microsoft.AspNetCore.SignalR.IHubContext<ImportExportManagementAPI.Hubs.TransactionHub> transHub)
        {
            TimedControlService = timedGenerateScheduleService;
            _transHub = transHub;
        }

        [HttpGet("stop")]
        public async Task<ActionResult> Stop()
        {
            await _transHub.Clients.All.SendAsync("SendMess", "123");
            //await TimedControlService.StopAsync(new System.Threading.CancellationToken());
            return Ok();
        }

        [HttpGet("start")]
        public async Task<ActionResult> Start()
        {
            await TimedControlService.StartAsync(new System.Threading.CancellationToken());
            return Ok();
        }
    }
}
