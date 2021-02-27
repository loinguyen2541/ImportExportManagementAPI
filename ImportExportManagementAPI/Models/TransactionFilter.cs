using ImportExportManagement_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Models
{
    public class TransactionFilter
    {
        public String DateCreate { get; set; }
        public String PartnerName { get; set; }
        public String TransactionType { get; set; }
    }
}
