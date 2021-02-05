using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagement_API.Models
{
    public class Inventory
    {
        public int InventoryId { get; set; }
        public DateTime RecordedDate { get; set; }
        public float Weight { get; set; }

        public List<InventoryDetail> InventoryDetails { get; set; }
    }
}
