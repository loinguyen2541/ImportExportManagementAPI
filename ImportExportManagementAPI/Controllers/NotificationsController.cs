using ImportExportManagementAPI.Enums;
using ImportExportManagementAPI.Filters;
using ImportExportManagementAPI.Hubs;
using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IHubContext<ScheduleHub> hubContext;
        NotificationRepository _repo;
        public NotificationsController(IHubContext<ScheduleHub> scheduleHub)
        {
            hubContext = scheduleHub;
            _repo = new NotificationRepository();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<Pagination<Notification>>> GetAllNotification([FromQuery] PaginationParam paging)
        {
            Pagination<Notification> notifications = await _repo.GetAllNotification(paging);
            return notifications;
        }
        [HttpGet("partners/{partnerId}")]
        [AllowAnonymous]
        public async Task<ActionResult<Pagination<Notification>>> GetPartnerNotification([FromQuery] PaginationParam paging, int partnerId)
        {
            Pagination<Notification> notifications = await _repo.GetNotificationPartner(paging, partnerId);
            return notifications;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Notification>> GetNotification(int id)
        {
            var noti = await _repo.GetByIDAsync(id);

            if (noti == null)
            {
                return NotFound();
            }

            return noti;
        }
        [HttpGet("partners")]
        [AllowAnonymous]
        public async Task<IActionResult> MakeNotificationAvailable([FromQuery] NotificationFilter filter)
        {
            await _repo.MakeNotificationAvailable(filter);
            return Ok();
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> PutNotification(int id, Notification noti)
        {
            if (id != noti.NotificationId)
            {
                return BadRequest();
            }
            _repo.Update(noti);

            try
            {
                await _repo.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return Ok(noti);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Notification>> PostNotification(Notification noti)
        {
            _repo.Insert(noti);
            await _repo.SaveAsync();
            await hubContext.Clients.All.SendAsync("PushNotiSuccess", "reload");
            return CreatedAtAction("GetNotification", new { id = noti.NotificationId }, noti);
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var noti = await _repo.GetByIDAsync(id);
            if (noti == null)
            {
                return NotFound();
            }

            noti.StatusPartner = NotificationStatus.Unavailable;
            _repo.Update(noti);
            await _repo.SaveAsync();

            return NoContent();
        }
    }
}
