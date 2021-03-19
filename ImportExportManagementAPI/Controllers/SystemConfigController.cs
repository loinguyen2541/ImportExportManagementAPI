﻿using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.Repositories;
using ImportExportManagementAPI.Workers;
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
        private readonly TimedGenerateScheduleService _timedGenerateScheduleService;

        public SystemConfigController(SystemConfigRepository systemConfigRepository, TimedGenerateScheduleService timedGenerateScheduleService)
        {
            _systemConfigRepository = systemConfigRepository;
            _timedGenerateScheduleService = timedGenerateScheduleService;
        }

        [HttpGet("storge-capacity")]
        public ActionResult<String> GetStorgeCapacity()
        {
            return Ok(_systemConfigRepository.GetStorageCapacity());
        }

        [HttpPut("auto-schedule")]
        public async Task<ActionResult<SystemConfig>> PutAutoSchedule(String time)
        {
            if (await _systemConfigRepository.UpdateAutoSchedule(time))
            {
                await _timedGenerateScheduleService.StopAsync(new System.Threading.CancellationToken());
                await _timedGenerateScheduleService.StartAsync(new System.Threading.CancellationToken());
                return NoContent();
            }
            return BadRequest();
        }

    }
}
