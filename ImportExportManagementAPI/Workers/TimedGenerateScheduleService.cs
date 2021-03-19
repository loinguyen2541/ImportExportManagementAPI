using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

/**
* @author Loi Nguyen
*
* @date - 3/9/2021 10:45:36 PM 
*/

namespace ImportExportManagementAPI.Workers
{
    public class TimedGenerateScheduleService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TimedGenerateScheduleService> _logger;
        private Timer _timer;
        private readonly TimeTemplateRepository _timeTemplateRepository;
        private readonly GoodsRepository _goodsRepository;
        private readonly ScheduleRepository _scheduleRepository;
        private readonly SystemConfigRepository _systemConfigRepository;

        public TimedGenerateScheduleService(ILogger<TimedGenerateScheduleService> logger)
        {
            _logger = logger;
            _timeTemplateRepository = new TimeTemplateRepository();
            _goodsRepository = new GoodsRepository();
            _scheduleRepository = new ScheduleRepository();
            _systemConfigRepository = new SystemConfigRepository();
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            DateTime now = DateTime.Now;

            SystemConfig scheduleTime = _systemConfigRepository.GetByID(AttributeKey.AutoSchedule.ToString());
            TimeSpan ts;

            if (TimeSpan.TryParse(scheduleTime.AttributeValue, out ts))
            {
                _logger.LogInformation(ts.Hours + "");
            }
            else
            {
                ts = new TimeSpan();
            }

            TimeTemplate currentTimeTemplate = _timeTemplateRepository.GetCurrentTimeTemplate();

            TimeSpan timeToGo = CalculateTimeToGo(ts, currentTimeTemplate.ApplyingDate);

            TimeSpan timeFrom = TimeSpan.FromDays(1);
            if (timeToGo == TimeSpan.Zero)
            {
                timeFrom = TimeSpan.FromDays(1) - (now.TimeOfDay - ts);
            }

            _timer = new Timer(DoWork, null, timeToGo, timeFrom);

            return Task.CompletedTask;
        }

        /// <summary>
        ///    This method calculates launch time of timer. <para/>
        ///    1. If the current date is less than the applied date <para/>
        ///                    => executes the timer at the scheduled time of the next day.<para/>
        ///    2. If the current date is equal to the applied date <para/>
        ///                     2.1 If current time is greater than or equal to scheduled time <para/>
        ///                     => executes the timer immediately.<para/>
        ///                     2.2 Else <para/>
        ///                     => excute the timer at scheduled time.<para/>
        ///    3. If the current date is greater than the applied date<para/>
        ///                     => executes the timer immediately.<para/>
        /// </summary>
        /// <param name="scheduleTime"> daily launch time. </param>
        /// <param name="applyingDate"> date applied for time template items.</param>
        /// <returns>launch time of timer</returns>
        private TimeSpan CalculateTimeToGo(TimeSpan scheduleTime, DateTime applyingDate)
        {
            TimeSpan timeToGo = new TimeSpan();
            DateTime currentDate = DateTime.Now;

            if (currentDate.Date < applyingDate.Date)
            {
                timeToGo = TimeSpan.FromDays(1) - (currentDate.TimeOfDay - scheduleTime);
            }
            else if (currentDate.Date == applyingDate.Date)
            {
                if (currentDate.TimeOfDay >= scheduleTime)
                {
                    timeToGo = TimeSpan.Zero;
                }
                else
                {
                    timeToGo = scheduleTime - currentDate.TimeOfDay;
                }
            }
            else
            {
                timeToGo = TimeSpan.Zero;
            }
            return timeToGo;
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            _logger.LogInformation(
                "Timed Hosted Service is working. Count: {Count}", count);
            float capacity = _goodsRepository.GetGoodCapacity();
            _timeTemplateRepository.ResetTimeTemplate(capacity);
            _scheduleRepository.DisableAll();
            if (count == 2)
            {
                _timer?.Change(TimeSpan.FromDays(1), TimeSpan.FromDays(1));
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
