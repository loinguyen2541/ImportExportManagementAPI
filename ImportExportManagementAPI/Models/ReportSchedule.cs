using ImportExportManagement_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Models
{
    public class ReportSchedule
    {
        public Partner partner { get; set; }
        public int totalSchedule { get; set; }
        public float totalRegisterImport { get; set; }
        public float totalRegisterExport { get; set; }
        public float totalRealWeightImport { get; set; }
        public float totalRealWeightExport { get; set; }
    }
}
