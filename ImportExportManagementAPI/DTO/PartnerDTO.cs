using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagement_API.DTO
{
    public class PartnerDTO
    {
        public int PartnerId { get; set; }
        [MaxLength(50)]
        public String DisplayName { get; set; }
    }
}
