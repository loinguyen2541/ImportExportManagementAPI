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

        public TimedSchedulesController(TimedGenerateScheduleService timedGenerateScheduleService)
        {
            TimedControlService = timedGenerateScheduleService;
        }

        [HttpGet("stop")]
        public async Task<ActionResult> Stop()
        {
            await TimedControlService.StopAsync(new System.Threading.CancellationToken());
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
