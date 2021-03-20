﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportExportManagement_API;
using ImportExportManagement_API.Models;
using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/schedules")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly ScheduleRepository _repo;
        private readonly TimeTemplateItemRepository _timeTemplateItemRepo;
        private readonly GoodsRepository _goodsRepository;
        private readonly SystemConfigRepository _systemConfigRepository;
        public SchedulesController(SystemConfigRepository systemConfigRepository)
        {
            _repo = new ScheduleRepository();
            _timeTemplateItemRepo = new TimeTemplateItemRepository();
            _goodsRepository = new GoodsRepository();
            _systemConfigRepository = systemConfigRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<Pagination<Schedule>>> GetScheduleByPartnerId(int partnerId)
        {
            List<Schedule> schedules = await _repo.GetByPartnerId(partnerId);
            return Ok(schedules);
        }

        // GET: api/Schedules/search
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<Pagination<Schedule>>> SearchSchedule([FromQuery] PaginationParam paging, [FromQuery] ScheduleFilterParam filter)
        {
            Pagination<Schedule> schedules = await _repo.GetAllAsync(paging, filter);
            return Ok(schedules);
        }
        [HttpGet("schedulehistory")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Schedule>>> GetHistorySchedule(String searchDate)
        {
            List<Schedule> schedules = await _repo.GetHistory(searchDate);
            return Ok(schedules);
        }

        // GET: api/Schedules/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Schedule>> GetSchedule(int id)
        {
            var schedule = await _repo.GetByIDAsync(id);

            if (schedule == null)
            {
                return NotFound();
            }

            return Ok(schedule);
        }

        // PUT: api/Schedules/5
        [HttpPut("changeschedule/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangeSchedule(int id, Schedule updateSchedule)
        {
            Schedule beforeSchedule = await _repo.GetByIDAsync(id);
            Schedule schedule = await _timeTemplateItemRepo.ChangeSchedule(updateSchedule, beforeSchedule);
            if (schedule != null)
            {
                try
                {
                    if (!_repo.TryToUpdate(schedule))
                    {
                        _repo.Insert(schedule);
                    }
                    await _repo.SaveAsync();
                    return CreatedAtAction("GetSchedule", new { id = schedule.ScheduleId }, schedule);
                }

                catch
                {
                    return BadRequest("Update failed");
                }

            }
            return BadRequest("Update failed");
        }

        // POST: api/Schedules
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Schedule>> PostSchedule(Schedule schedule)
        {
            float storgeCapacity;
            if (!float.TryParse(_systemConfigRepository.GetStorageCapacity(), out storgeCapacity))
            {
                return NoContent();
            }
            if (_timeTemplateItemRepo.CheckInventory(schedule.RegisteredWeight, schedule.TimeTemplateItemId, schedule.TransactionType, storgeCapacity))
            {
                _timeTemplateItemRepo.UpdateCurrent(schedule.TransactionType, schedule.RegisteredWeight, schedule.TimeTemplateItemId);
                schedule.IsCanceled = false;
                if (!_repo.TryToUpdate(schedule))
                {
                    _repo.Insert(schedule);
                }
                await _repo.SaveAsync();
                return CreatedAtAction("GetSchedule", new { id = schedule.ScheduleId }, schedule);
            }
            return BadRequest();
        }

        [HttpPut("cancel")]
        [AllowAnonymous]
        public async Task<ActionResult<Schedule>> CancelSchedule(int id, String username)
        {
            Schedule schedule = _repo.GetByID(id);
            if (schedule != null)
            {
                if (schedule.IsCanceled == false)
                {
                    bool checkCancel = await _timeTemplateItemRepo.CancelSchedule(schedule, username);
                    await _repo.SaveAsync();
                    return NoContent();
                }
            }
            return NotFound();
        }
    }
}
