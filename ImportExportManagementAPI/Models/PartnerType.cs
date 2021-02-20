using ImportExportManagement_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/**
* @author Loi Nguyen
*
* @date - 2/19/2021 9:54:43 PM 
*/

namespace ImportExportManagementAPI.Models
{
    public class PartnerType
    {
        public int PartnerTypeId { get; set; }
        public String PartnerTypeName { get; set; }

        public List<Partner> Partners { get; set; }
    }
}
