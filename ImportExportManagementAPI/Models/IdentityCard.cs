using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

/**
* @author Loi Nguyen
*
* @date - 1/30/2021 12:53:45 PM 
*/

namespace ImportExportManagement_API.Models
{
    public class IdentityCard
    {
        public String IdentityCardId { get; set; }
        public IdentityCardStatus IdentityCardStatus { get; set; }

        public int PartnerId { get; set; }
        public Partner Partner { get; set; }
    }
}
