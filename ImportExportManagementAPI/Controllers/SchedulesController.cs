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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSchedule(int id, Schedule schedule)
        {
            if (id != schedule.ScheduleId)
            {
                return BadRequest();
            }

            _repo.Update(schedule);

            try
            {
                await _repo.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_repo.Exist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Schedules
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // DELETE: api/Schedules/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _repo.GetByIDAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            _repo.Delete(schedule);
            await _repo.SaveAsync();

            return NoContent();
        }

        [HttpGet("types")]
        public ActionResult<Object> GetTransType()
        {
            return Ok(Enum.GetValues(typeof(TransactionType)).Cast<TransactionType>().ToList());
        }

        [HttpGet("time")]
        public ActionResult<Object> ChangeTime()
        {
            return Ok(Enum.GetValues(typeof(TransactionType)).Cast<TransactionType>().ToList());
        }
    }
}
