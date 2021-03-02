using ImportExportManagementAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/**
* @author Loi Nguyen
*
* @date - 2/4/2021 5:03:03 PM 
*/

namespace ImportExportManagement_API.Models
{
    public class InventoryDetail
    {
        public int GoodsId { get; set; }
        public Goods Goods { get; set; }

        public float Weight { get; set; }
        public int InventoryId { get; set; }
        public InventoryDetailType Type { get; set; }
        public Inventory Inventory { get; set; }
    }
}
