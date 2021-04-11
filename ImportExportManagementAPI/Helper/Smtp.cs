using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Helper
{
    public class Smtp
    {
        public string username { get; set; }
        public string password { get; set; }
        public int port { get; set; }
        public string host { get; set; }
    }
}
