using ImportExportManagement_API.Models;
using ImportExportManagementAPI.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public NotificationType NotificationType { get; set; }
        public NotificationStatus Status { get; set; }
        public String Title { get; set; }
        public String Content { get; set; }
        public DateTime CreatedDate { get; set; }

        [MaxLength(50)]
        public String Username { get; set; }
        public Account Account { get; set; }
    }
}
