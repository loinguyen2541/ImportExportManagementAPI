using ImportExportManagement_API.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Repositories;
using ImportExportManagementAPI.Hubs;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace ImportExportManagementAPI.Workers
{
    public class UpdateScheduleQueueService : BackgroundService
    {
        public Queue<Schedule> Schedules;
        private readonly ILogger<UpdateScheduleQueueService> _logger;
        private readonly ScheduleRepository _repo;
        private readonly TimeTemplateItemRepository _timeTemplateItemRepo;
        private readonly GoodsRepository _goodsRepository;
        private readonly SystemConfigRepository _systemConfigRepository;
        private readonly IHubContext<ScheduleHub> hubContext;

        public UpdateScheduleQueueService(ILogger<UpdateScheduleQueueService> logger, IHubContext<ScheduleHub> scheduleHub)
        {
            _logger = logger;
            Schedules = new Queue<Schedule>();
            _repo = new ScheduleRepository();
            _timeTemplateItemRepo = new TimeTemplateItemRepository();
            _goodsRepository = new GoodsRepository();
            _systemConfigRepository = new SystemConfigRepository();
            hubContext = scheduleHub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (Schedules.Count > 0)
                {
                    await UpdateSchduleAsync(Schedules.Dequeue());
                    _logger.LogInformation("Task: Excuted");
                }
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task UpdateSchduleAsync(Schedule schedule)
        {
            if (schedule == null)
            {
                SendMessage(CreateMessage(schedule.PartnerId, CreateSchuleResponseStatus.Error, "Your schedule not valid!!"));
                return;
            }
            float storgeCapacity;
            if (!float.TryParse(_systemConfigRepository.GetStorageCapacity(), out storgeCapacity))
            {
                SendMessage(CreateMessage(schedule.PartnerId, CreateSchuleResponseStatus.Error, "System error!"));
            }
            Boolean checkTime = _timeTemplateItemRepo.CheckValidTime(schedule.TimeTemplateItemId);
            if (checkTime)
            {
                if (schedule.RegisteredWeight > 0)
                {
                    if (await _timeTemplateItemRepo.CheckInventory(schedule.RegisteredWeight, schedule.TimeTemplateItemId, schedule.TransactionType, storgeCapacity))
                    {
                        try
                        {
                            _repo.UpdateForWorker(schedule);
                  
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            SendMessage(CreateMessage(schedule.PartnerId, CreateSchuleResponseStatus.Error, "Server is error. Try again later!"));
                        }

                        SendMessage(CreateMessage(schedule.PartnerId, CreateSchuleResponseStatus.Success, "You have update Schedule successfully!"));
                        return;
                    }
                    SendMessage(CreateMessage(schedule.PartnerId, CreateSchuleResponseStatus.Error, "Your registered weight exceeds the stock volume!"));
                    return;
                }
                SendMessage(CreateMessage(schedule.PartnerId, CreateSchuleResponseStatus.Error, "Your registered weight must be greater than 0!"));
                return;
            }
            SendMessage(CreateMessage(schedule.PartnerId, CreateSchuleResponseStatus.Error, "The period you have registered is not valid!"));
            return;
        }

        public void SendMessage(String message)
        {
            hubContext.Clients.All.SendAsync("UpdateSchedule", message);
            _logger.LogInformation("Log message: " + message);
        }

        public String CreateMessage(int partnerid, CreateSchuleResponseStatus status, String message)
        {
            UpdatSchuleResponseMessage createSchuleResponseMessage = new UpdatSchuleResponseMessage();
            createSchuleResponseMessage.PartnerId = partnerid;
            createSchuleResponseMessage.Status = status;
            createSchuleResponseMessage.Message = message;
            String result = JsonConvert.SerializeObject(createSchuleResponseMessage);
            return result;
        }
    }

    public class UpdatSchuleResponseMessage
    {
        public int PartnerId { get; set; }
        public CreateSchuleResponseStatus Status { get; set; }
        public String Message { get; set; }
    }


}

