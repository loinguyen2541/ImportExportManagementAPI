using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Models
{
    public class TotalInventoryDetailedByDate
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public float weight { get; set; }
    }
}
