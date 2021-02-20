using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagement_API.Models
{
    public class Transaction
    {
        public enum TransactionTypeX
        {
            Import, Export
        }
        public int TransactionId { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public float WeightIn { get; set; }
        public float WeightOut { get; set; }
        public TransactionTypeX TransactionType { get; set; }
        public TransactionStatus TransactionStatus { get; set; }

        public int PartnerId { get; set; }
        public Partner Partner { get; set; }

        public int GoodsId { get; set; }
        public Goods Goods { get; set; }

        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; }
    }
}
