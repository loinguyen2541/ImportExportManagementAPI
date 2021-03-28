using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/**
* @author Loi Nguyen
*
* @date - 3/28/2021 4:17:16 PM 
*/

namespace ImportExportManagementAPI.Objects
{
    public class InventoryTotalImportExport
    {
        public int InventoryId { get; set; }
        public DateTime RecordedDate { get; set; }
        public float OpeningStock { get; set; }
        public float TotalImport { get; set; }
        public float TotalExport { get; set; }
        public float ClosingStock { get; set; }
    }
}
