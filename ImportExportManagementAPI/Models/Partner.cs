using ImportExportManagementAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagement_API.Models
{
    public class Partner
    {
        public int PartnerId { get; set; }

        [MaxLength(50)]
        public String DisplayName { get; set; }

        [MaxLength(50)]
        public String Email { get; set; }

        [MaxLength(10)]
        public String PhoneNumber { get; set; }

        [MaxLength(200)]
        public String Address { get; set; }
        public PartnerStatus PartnerStatus { get; set; }

        public List<IdentityCard> IdentityCards { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Schedule> Schedules { get; set; }

        public String Username { get; set; }
        public Account Account { get; set; }

        public List<PartnerType> PartnerTypes { get; set; }

    }
}
