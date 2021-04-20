using ImportExportManagement_API.Models;
using ImportExportManagementAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public NotificationType NotificationType { get; set; }
        public NotificationStatus StatusAdmin { get; set; }
        public NotificationStatus StatusPartner { get; set; }
        public String ContentForAdmin { get; set; }
        public String ContentForPartner { get; set; }
        public DateTime CreatedDate { get; set; }
        public int PartnerId { get; set; }
        public Partner Partner { get; set; }
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public Schedule Schedule { get; set; }
    }
}
