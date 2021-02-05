using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagement_API.Models
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public DateTime ScheduleDate { get; set; }
        public float RegisteredWeight { get; set; }
        public TransactionType TransactionType { get; set; }
        public bool IsCanceled { get; set; }

        public int PartnerId { get; set; }
        public Partner Partner { get; set; }

        public int GoodsId { get; set; }
        public Goods Goods { get; set; }

    }
}
