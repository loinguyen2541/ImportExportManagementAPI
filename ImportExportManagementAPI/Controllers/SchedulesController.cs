using System;
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

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/schedules")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly ScheduleRepository _repo;
        private readonly TimeTemplateItemRepository _timeTemplateItemRepo;
        private readonly GoodsRepository _goodsRepository;
        public SchedulesController()
        {
            _repo = new ScheduleRepository();
            _timeTemplateItemRepo = new TimeTemplateItemRepository();
            _goodsRepository = new GoodsRepository();
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<Schedule>>> GetScheduleByPartnerId(int partnerId)
        {
            List<Schedule> schedules = await _repo.GetByPartnerId(partnerId);
            return Ok(schedules);
        }

        // GET: api/Schedules/search
        [HttpGet("search")]
        public async Task<ActionResult<Pagination<Schedule>>> SearchSchedule([FromQuery] PaginationParam paging, [FromQuery] ScheduleFilterParam filter)
        {
            Pagination<Schedule> schedules = await _repo.GetAllAsync(paging, filter);
            return Ok(schedules);
        }
        [HttpGet("schedulehistory")]
        public async Task<ActionResult<List<Schedule>>> GetHistorySchedule(String searchDate)
        {
            List<Schedule> schedules = await _repo.GetHistory(searchDate);
            return Ok(schedules);
        }

        // GET: api/Schedules/5
        [HttpGet("{id}")]
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
        public async Task<IActionResult> ChangeSchedule(int id, int time)
        {
            Schedule scheduleBefore = _repo.GetByID(id);
            Schedule scheduleUpdate = _repo.GetByID(id);
            if (scheduleBefore != null)
            {
                if (scheduleBefore.IsCanceled == true)
                {
                    return BadRequest();
                }
                else
                {
                    //change inventory and timeitem
                    bool checkUpdate = await _timeTemplateItemRepo.ChangeSchedule(scheduleUpdate, scheduleBefore);
                    if (checkUpdate)
                    {
                        scheduleUpdate.TimeTemplateItemId = 11;
                        try
                        {
                            await _repo.SaveAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            BadRequest();
                        }
                    }
                }
            }
            return NoContent();
        }

        // POST: api/Schedules
        [HttpPost]
        public async Task<ActionResult<Schedule>> PostSchedule(Schedule schedule)
        {

            if (_timeTemplateItemRepo.CheckCapacity(schedule.RegisteredWeight, schedule.TimeTemplateItemId))
            {
                // float quantityOfInventory = _goodsRepository.GetGoodCapacity(schedule.GoodsId);
                _timeTemplateItemRepo.UpdateSchedule(schedule.TransactionType, schedule.RegisteredWeight, schedule.TimeTemplateItemId);
                schedule.IsCanceled = false;
                if (!_repo.TryToUpdate(schedule))
                {
                    _repo.Insert(schedule);
                }
                await _repo.SaveAsync();
            }
            return CreatedAtAction("GetSchedule", new { id = schedule.ScheduleId }, schedule);
        }

        [HttpPut("cancel")]
        public async Task<ActionResult<Schedule>> CancelSchedule(int id, String username)
        {
            Schedule schedule = _repo.GetByID(id);
            if (schedule != null)
            {
                if (schedule.IsCanceled == false)
                {
                    bool checkCancel = await _timeTemplateItemRepo.CancelSchedule(schedule);
                    if (checkCancel)
                    {
                        schedule.IsCanceled = true;
                        schedule.UpdatedBy = username;
                        _repo.Update(schedule);
                    }
                    try
                    {
                        await _repo.SaveAsync();
                    }
                    catch
                    {
                        return BadRequest();
                    }

                    return NoContent();
                }
            }
            return NotFound();
        }
    }
}
