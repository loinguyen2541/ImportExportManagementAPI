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
        public String DateFrom { get; set; }
        public String DateTo { get; set; }
        public String PartnerName { get; set; }
        public String TransactionType { get; set; }
        public int PartnerId { get; set; }
        public String TransactionStatus { get; set; }
    }
}
