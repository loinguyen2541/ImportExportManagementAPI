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
    public class Goods
    {
        public int GoodsId { get; set; }

        [MaxLength(50)]
        public String GoodName { get; set; }
        public float QuantityOfInventory { get; set; }
        public GoodsStatus GoodsStatus { get; set; }

        public List<Transaction> Transactions { get; set; }

        public List<Schedule> Schedules { get; set; }

        public List<InventoryDetail> InventoryDetails { get; set; }
    }
}
