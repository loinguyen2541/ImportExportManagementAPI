using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagement_API.Models
{
    public class InventoryFilter
    {
        public String PartnerName { get; set; }
        public String TransactionType { get; set; }
        public String RecordedDate { get; set; }
        public String dateTo { get; set; }
        public String dateFrom { get; set; }
    }
}
