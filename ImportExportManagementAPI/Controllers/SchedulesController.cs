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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ImportExportManagementAPI.Hubs;
using ImportExportManagementAPI.Workers;

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/schedules")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly ScheduleRepository _repo;
        private readonly TimeTemplateItemRepository _timeTemplateItemRepo;
        private readonly IHubContext<ScheduleHub> hubContext;
        private readonly CreateScheduleQueueService _createScheduleQueueService;
        private readonly UpdateScheduleQueueService _updateScheduleQueueService;

        public SchedulesController(IHubContext<ScheduleHub> scheduleHub, CreateScheduleQueueService createScheduleQueueService,
           UpdateScheduleQueueService updateScheduleQueueService)
        {
            _repo = new ScheduleRepository();
            _timeTemplateItemRepo = new TimeTemplateItemRepository();
            hubContext = scheduleHub;
            _createScheduleQueueService = createScheduleQueueService;
            _updateScheduleQueueService = updateScheduleQueueService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<Pagination<Schedule>>> GetScheduleByPartnerId(int partnerId)
        {
            List<Schedule> schedules = await _repo.GetByPartnerId(partnerId);
            return Ok(schedules);
        }

        [HttpGet("getAll")]
        [AllowAnonymous]
        public ActionResult<Pagination<Schedule>> GetScheduleToDay()
        {
            List<Schedule> schedules = _repo.GetAllAsyncToday();
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
        public async Task<ActionResult<List<Schedule>>> GetHistorySchedule(String searchDate, int partnerId)
        {
            List<Schedule> schedules = await _repo.GetHistory(searchDate, partnerId);
            return Ok(schedules);
        }

        // GET: api/Schedules/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Schedule>> GetSchedule(int id)
        {
            var schedule = await _repo.GetScheduleById(id);

            if (schedule == null)
            {
                return NotFound();
            }

            return Ok(schedule);
        }

        // PUT: api/Schedules/5
        [HttpPut("changeschedule/{id}")]
        [AllowAnonymous]
        public IActionResult ChangeSchedule(int id, Schedule updateSchedule)
        {
            if (id != updateSchedule.ScheduleId)
            {
                return BadRequest();
            }
            if (_timeTemplateItemRepo.CheckValidTime(updateSchedule.TimeTemplateItemId))
            {
                _updateScheduleQueueService.Schedules.Enqueue(updateSchedule);
            }
            else
            {
                return BadRequest("This frame is time out");
            }
            return Ok();
        }

        // POST: api/Schedules
        [HttpPost]
        [AllowAnonymous]
        public ActionResult<Schedule> PostSchedule(Schedule schedule)
        {
            _createScheduleQueueService.Schedules.Enqueue(schedule);
            return Ok();
            //float storgeCapacity;
            //if (!float.TryParse(_systemConfigRepository.GetStorageCapacity(), out storgeCapacity))
            //{
            //    return NoContent();
            //}
            //Boolean checkTime = _timeTemplateItemRepo.CheckValidTime(schedule.TimeTemplateItemId);
            //if (checkTime)
            //{
            //    if (schedule.RegisteredWeight != 0)
            //    {
            //        TransactionType type = _timeTemplateItemRepo.DefineTransactionType(schedule.PartnerId);
            //        schedule.TransactionType = type;

            //        if (_timeTemplateItemRepo.CheckInventory(schedule.RegisteredWeight, schedule.TimeTemplateItemId, schedule.TransactionType, storgeCapacity))
            //        {
            //            if (_timeTemplateItemRepo.UpdateCurrent(schedule.TransactionType, schedule.RegisteredWeight, schedule.TimeTemplateItemId))
            //            {

            //                //check date
            //                String scheduleTime = _systemConfigRepository.GetAutoSchedule();
            //                DateTime generateScheduleTime = DateTime.Parse(scheduleTime);
            //                DateTime current = DateTime.Now;
            //                if (current > generateScheduleTime)
            //                {
            //                    schedule.ScheduleDate = schedule.ScheduleDate.AddDays(1);
            //                }
            //                schedule.ScheduleStatus = ScheduleStatus.Approved;
            //                _repo.Insert(schedule);
            //                _repo.Save();
            //                Task.Run(new Action(() =>
            //                {
            //                    hubContext.Clients.All.SendAsync("ReloadScheduleList", "reload");
            //                }));
            //                return CreatedAtAction("GetSchedule", new { id = schedule.ScheduleId }, schedule);
            //            }
            //            return BadRequest("Inventory is full");
            //        }
            //        return BadRequest("Inventory is full");
            //    }
            //    return BadRequest("Weight must be greater than 0");
            //}
            //return BadRequest("Out of time to schedule");
        }

        [HttpPut("cancel")]
        [AllowAnonymous]
        public async Task<ActionResult<Schedule>> CancelSchedule(int id, String username)
        {
            Schedule schedule = _repo.GetByID(id);
            if (schedule != null)
            {
                if (schedule.ScheduleStatus == ScheduleStatus.Approved)
                {
                    Schedule checkCancel = await _timeTemplateItemRepo.CancelSchedule(schedule, username);
                    _repo.Update(checkCancel);
                    await _repo.SaveAsync();
                    await hubContext.Clients.All.SendAsync("CancelSchedule", "other user cancel schedule");
                    return NoContent();
                }
                else
                {
                    return BadRequest();
                }
            }
            return NotFound();
        }

        [HttpGet("top10")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Schedule>>> GetTop10Schedule([FromQuery] string partnerID)
        {
            return Ok(await _repo.GetTop10Schedule(partnerID));
        }
        [HttpGet("search-partner")]
        [AllowAnonymous]
        public ActionResult<Pagination<Schedule>> GetScheduleByPartner([FromQuery] ScheduleFilterParam filter, [FromQuery] PaginationParam paging)
        {
            Pagination<Schedule> schedules = _repo.DoFilterSearchPartner(filter, paging);
            return Ok(schedules);
        }

        [HttpGet("count-total")]
        [AllowAnonymous]
        public int GetCountTotal(int type)
        {
            int count = _repo.GetTotalByType(type);
            return count;
        }
        [HttpGet("getScheduleStatus")]
        [AllowAnonymous]
        public List<ScheduleStatus> getScheduleStatus()
        {
            List<ScheduleStatus> scheduleStatuses = _repo.getScheduleType();
            return scheduleStatuses;
        }
        [HttpGet("total-schedule")]
        [AllowAnonymous]
        public async Task<float> GetTotalScheduleAsync(int partnerId, String searchDate)
        {
            float totalWeight = await _repo.GetTotalSchedule(partnerId, searchDate);
            return totalWeight;
        }
    }
}
