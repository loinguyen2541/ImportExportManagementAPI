using ImportExportManagement_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Models
{
    public class ActivityLog
    {
        public int ActivityLogId { get; set; }
        public String Username { get; set; }
        public DateTime RecordDate { get; set; }
        public String Description { get; set; }
        public Account Account { get; set; }
    }
}
