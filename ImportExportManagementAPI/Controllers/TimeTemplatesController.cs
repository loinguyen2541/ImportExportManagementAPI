using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportExportManagementAPI.Models;
using ImportExportManagement_API;
using ImportExportManagementAPI.Repositories;

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/timetemplates")]
    [ApiController]
    public class TimeTemplatesController : ControllerBase
    {
        private readonly IEDbContext _context;
        private readonly TimeTemplateRepository _timeTemplateRepository;

        public TimeTemplatesController(IEDbContext context)
        {
            _context = context;
            _timeTemplateRepository = new TimeTemplateRepository();
        }

        // GET: api/TimeTemplates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimeTemplate>>> GetTimeTemplate()
        {
            return await _context.TimeTemplate.ToListAsync();
        }

        // GET: api/TimeTemplates/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TimeTemplate>> GetTimeTemplate(int id)
        {
            var timeTemplate = await _context.TimeTemplate.FindAsync(id);

            if (timeTemplate == null)
            {
                return NotFound();
            }

            return timeTemplate;
        }

        // PUT: api/TimeTemplates/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTimeTemplate(int id, TimeTemplate timeTemplate)
        {
            if (id != timeTemplate.TimeTemplateId)
            {
                return BadRequest();
            }

            _context.Entry(timeTemplate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimeTemplateExists(id))
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

        // POST: api/TimeTemplates
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TimeTemplate>> PostTimeTemplate(TimeTemplate timeTemplate)
        {
            _context.TimeTemplate.Add(timeTemplate);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTimeTemplate", new { id = timeTemplate.TimeTemplateId }, timeTemplate);
        }

        // DELETE: api/TimeTemplates/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeTemplate(int id)
        {
            var timeTemplate = await _context.TimeTemplate.FindAsync(id);
            if (timeTemplate == null)
            {
                return NotFound();
            }

            _context.TimeTemplate.Remove(timeTemplate);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("current")]
        public async Task<ActionResult<TimeTemplate>> GetCurrentTimeTemplate(int partnerId)
        {
            TimeTemplate timeTemplate = await _timeTemplateRepository.GetCurrentTimeTemplateAsync(partnerId);
            return timeTemplate;
        }

        private bool TimeTemplateExists(int id)
        {
            return _context.TimeTemplate.Any(e => e.TimeTemplateId == id);
        }
    }
}
