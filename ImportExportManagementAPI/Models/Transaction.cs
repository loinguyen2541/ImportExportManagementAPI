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

        [AllowNull]
        public DateTime TimeOut { get; set; }
        public float WeightIn { get; set; }
        public DateTime CreatedDate { get; set; }

        [AllowNull]
        public float WeightOut { get; set; }

        [AllowNull]
        public TransactionType TransactionType { get; set; }
        public TransactionStatus TransactionStatus { get; set; }

        public String IdentityCardId { get; set; }
        public IdentityCard IdentityCard { get; set; }

        public int GoodsId { get; set; }
        public Goods Goods { get; set; }

    }
}
