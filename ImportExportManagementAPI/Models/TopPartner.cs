using ImportExportManagement_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace ImportExportManagementAPI.Models
{
    public class TopPartner
    {
       public Partner partner { get; set; }
       public float totalWeight { get; set; }
    }
}
