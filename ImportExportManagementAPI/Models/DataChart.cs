using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Models
{
    public class DataChart
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public float weightImport { get; set; }
        public float weightExport { get; set; }
    }
}
