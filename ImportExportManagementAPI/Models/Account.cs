using ImportExportManagementAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

/**
* @author Loi Nguyen
*
* @date - 2/3/2021 5:39:13 PM 
*/

namespace ImportExportManagement_API.Models
{
    public class Account
    {
        public String Username { get; set; }
        [MaxLength(25)]
        public String Password { get; set; }
        public AccountStatus Status { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
        public Partner Partner { get; set; }
    }
}
