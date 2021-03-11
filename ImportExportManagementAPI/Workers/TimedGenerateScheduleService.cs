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

        public TimedGenerateScheduleService(ILogger<TimedGenerateScheduleService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            DateTime now = DateTime.Now;
            DateTime firstRun = new DateTime(now.Year, now.Month, now.Day, 13, 5, 0, 0);
            //if (now > firstRun)
            //{
            //    firstRun = firstRun.AddDays(1);
            //}

            TimeSpan timeToGo = firstRun - now;
            if (timeToGo <= TimeSpan.Zero)
            {
                timeToGo = TimeSpan.Zero;
            }

            _timer = new Timer(DoWork, null, timeToGo,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            _logger.LogInformation(
                "Timed Hosted Service is working. Count: {Count}", count);
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
