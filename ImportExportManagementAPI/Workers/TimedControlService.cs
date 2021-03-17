using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/**
* @author Loi Nguyen
*
* @date - 3/17/2021 9:50:34 AM 
*/

namespace ImportExportManagementAPI.Workers
{
    public class TimedControlService
    {
        TimedGenerateScheduleService TimedGenerateScheduleService;

        public TimedControlService(TimedGenerateScheduleService timedGenerateScheduleService)
        {
            TimedGenerateScheduleService = timedGenerateScheduleService;
        }

        public async Task On()
        {
            await TimedGenerateScheduleService.StartAsync(new System.Threading.CancellationToken());

        }
        public async Task Off()
        {
            await TimedGenerateScheduleService.StartAsync(new System.Threading.CancellationToken());
        }
    }
}
