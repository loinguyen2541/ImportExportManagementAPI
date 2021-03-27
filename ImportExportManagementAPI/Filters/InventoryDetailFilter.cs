﻿using ImportExportManagementAPI.Models;
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
    public class InventoryDetailFilter
    {
        public String PartnerName { get; set; }
        public int GoodsId { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
