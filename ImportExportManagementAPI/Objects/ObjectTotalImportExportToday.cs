using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Objects
{
    public class ObjectTotalImportExportToday
    {
        public float OpeningStock { get; set; }
        public float Import{ get; set; } 
        public float Export { get; set; }
        public float Iventory { get; set; }
    }
}
