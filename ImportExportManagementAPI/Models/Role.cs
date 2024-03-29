﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/**
* @author Loi Nguyen
*
* @date - 2/3/2021 5:40:45 PM 
*/

namespace ImportExportManagement_API.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public String RoleName { get; set; }

        public List<Account> Accounts { get; set; }
    }
}
