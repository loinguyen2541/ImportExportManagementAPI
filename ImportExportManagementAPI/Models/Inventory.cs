using ImportExportManagementAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagement_API.Models
{
    public class Inventory
    {
        public int InventoryId { get; set; }

        [DataType(DataType.Date)]
        public DateTime RecordedDate { get; set; }

        public List<InventoryDetail> InventoryDetails { get; set; }
    }
}
