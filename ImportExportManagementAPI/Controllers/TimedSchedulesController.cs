using ImportExportManagementAPI.Workers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        CreateScheduleQueueService CreateScheduleQueueService;

        public TimedSchedulesController(TimedGenerateScheduleService timedGenerateScheduleService, CreateScheduleQueueService createScheduleQueueService)
        {
            TimedControlService = timedGenerateScheduleService;
            CreateScheduleQueueService = createScheduleQueueService;
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

        [HttpGet("add")]
        public ActionResult add()
        {
            CreateScheduleQueueService.Schedules.Enqueue(new ImportExportManagement_API.Models.Schedule());
            return Ok();
        }
    }
}
