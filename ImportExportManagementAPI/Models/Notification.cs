using ImportExportManagement_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int NotificationType { get; set; }
        public int StatusAdmin { get; set; }
        public int StatusPartner { get; set; }
        public String ContentForAdmin { get; set; }
        public String ContentForPartner { get; set; }
        public int PartnerId { get; set; }
        public Partner Partner { get; set; }
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
    }
}
