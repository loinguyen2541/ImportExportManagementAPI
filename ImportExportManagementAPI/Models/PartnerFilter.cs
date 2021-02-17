using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagement_API.Models
{
    public class PartnerFilter
    {
        [MaxLength(50)]
        public String Email { get; set; }

        [MaxLength(10)]
        public String PhoneNumber { get; set; }
        public PartnerStatus PartnerStatus { get; set; }
        [MaxLength(50)]
        public String DisplayName { get; set; }
    }
}
