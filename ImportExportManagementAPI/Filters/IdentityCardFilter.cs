﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Models
{
    public class IdentityCardFilter
    {
        public String PartnerName { get; set; }
        public int PartnerTypeId { get; set; }
        public String Status { get; set; }
    }
}