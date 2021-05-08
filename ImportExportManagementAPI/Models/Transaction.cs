using ImportExportManagementAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagement_API.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public float WeightIn { get; set; }
        public DateTime CreatedDate { get; set; }
        public String Description { get; set; }
        public float WeightOut { get; set; }
        public bool IsScheduled { get; set; }
        public TransactionType TransactionType { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public int PartnerId { get; set; }
        public Partner Partner { get; set; }

        public String IdentificationCode { get; set; }

        public int GoodsId { get; set; }
        public Goods Goods { get; set; }
    }
}
