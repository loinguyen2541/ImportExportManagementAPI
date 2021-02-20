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

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/schedules")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly ScheduleRepository _repo;

        public SchedulesController()
        {
            _repo = new ScheduleRepository();
        }

        // GET: api/Schedules
        [HttpGet]
        public async Task<ActionResult<Pagination<Schedule>>> GetSchedule([FromQuery] PaginationParam paging, [FromQuery] ScheduleFilterParam filter)
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
            _repo.Insert(schedule);
            await _repo.SaveAsync();

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
    }
}
