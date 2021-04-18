using ImportExportManagement_API.Models;
using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Hubs;
using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Workers
{
    public class CreateScheduleQueueService : BackgroundService
    {
        public Queue<Schedule> Schedules;
        private readonly ILogger<CreateScheduleQueueService> _logger;
        private readonly ScheduleRepository _repo;
        private readonly TimeTemplateItemRepository _timeTemplateItemRepo;
        private readonly GoodsRepository _goodsRepository;
        private readonly SystemConfigRepository _systemConfigRepository;
        private readonly IHubContext<ScheduleHub> hubContext;

        public CreateScheduleQueueService(ILogger<CreateScheduleQueueService> logger, IHubContext<ScheduleHub> scheduleHub)
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
                    CreateSchdule(Schedules.Dequeue());
                    _logger.LogInformation("Task: Excuted");
                }
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void CreateSchdule(Schedule schedule)
        {
            float storgeCapacity;
            if (!float.TryParse(_systemConfigRepository.GetStorageCapacity(), out storgeCapacity))
            {
                SendMessage(CreateMessage(schedule.PartnerId, CreateSchuleResponseStatus.Error, "System error!"));
            }
            Boolean checkTime = _timeTemplateItemRepo.CheckValidTime(schedule.TimeTemplateItemId);
            if (checkTime)
            {
                if (schedule.RegisteredWeight != 0)
                {
                    TransactionType type = _timeTemplateItemRepo.DefineTransactionType(schedule.PartnerId);
                    schedule.TransactionType = type;

                    if (_timeTemplateItemRepo.CheckInventory(schedule.RegisteredWeight, schedule.TimeTemplateItemId, schedule.TransactionType, storgeCapacity))
                    {
                        _timeTemplateItemRepo.UpdateCurrent(schedule.TransactionType, schedule.RegisteredWeight, schedule.TimeTemplateItemId);

                        //check date
                        String scheduleTime = _systemConfigRepository.GetAutoSchedule();
                        DateTime generateScheduleTime = DateTime.Parse(scheduleTime);
                        DateTime current = DateTime.Now;
                        if (current > generateScheduleTime)
                        {
                            schedule.ScheduleDate = schedule.ScheduleDate.AddDays(1);
                        }
                        schedule.ScheduleStatus = Models.ScheduleStatus.Approved;
                        _repo.Insert(schedule);
                        _repo.Save();
                        //Task.Run(new Action(() =>
                        //{
                        //    hubContext.Clients.All.SendAsync("ReloadScheduleList", "reload");
                        //}));
                        SendMessage(CreateMessage(schedule.PartnerId, CreateSchuleResponseStatus.Success, "You have successfully scheduled!"));
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
            hubContext.Clients.All.SendAsync("CreateResponse", message);
            _logger.LogInformation("Log message: " + message);
        }

        public String CreateMessage(int partnerid, CreateSchuleResponseStatus status, String message)
        {
            CreateSchuleResponseMessage createSchuleResponseMessage = new CreateSchuleResponseMessage();
            createSchuleResponseMessage.PartnerId = partnerid;
            createSchuleResponseMessage.Status = status;
            createSchuleResponseMessage.Message = message;
            String result = JsonConvert.SerializeObject(createSchuleResponseMessage);
            return result;
        }
    }

    public class CreateSchuleResponseMessage
    {
        public int PartnerId { get; set; }
        public CreateSchuleResponseStatus Status { get; set; }
        public String Message { get; set; }
    }

    public enum CreateSchuleResponseStatus
    {
        Success, Error
    }

}
