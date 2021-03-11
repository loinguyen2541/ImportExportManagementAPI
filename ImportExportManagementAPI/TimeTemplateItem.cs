using ImportExportManagement_API.Models;
using ImportExportManagementAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/**
* @author Loi Nguyen
*
* @date - 3/11/2021 11:06:25 AM 
*/

namespace ImportExportManagementAPI
{
    public class TimeTemplateItem
    {
        public int TimeTemplateItemId { get; set; }
        public float Capacity { get; set; }
        public DateTime ScheduleTime { get; set; }

        public List<Schedule> Schedules { get; set; }

        public int TimeTemplateId { get; set; }
        public TimeTemplate TimeTemplate { get; set; }
    }
}
