using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/**
* @author Loi Nguyen
*
* @date - 2/5/2021 11:02:39 AM 
*/

namespace ImportExportManagement_API.Models
{
    public class ScheduleFilter
    {
        public String TransactionType { get; set; }
        public String PartnerName { get; set; }
        public String ScheduleDate { get; set; }
    }
}
