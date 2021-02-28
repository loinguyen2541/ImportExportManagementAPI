using ImportExportManagement_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/**
* @author Loi Nguyen
*
* @date - 2/22/2021 12:40:35 AM 
*/

namespace ImportExportManagementAPI.Models
{
    public class PartnerPartnerType
    {
        public int PartnerId { get; set; }
        public Partner Partner { get; set; }

        public int PartnerTypeId { get; set; }
        public PartnerType PartnerType { get; set; }
    }
}
