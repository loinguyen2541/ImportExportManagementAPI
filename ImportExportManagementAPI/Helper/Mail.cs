using ImportExportManagement_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Helper
{
    public class Mail
    {
        public string subject { get; set; }
        public string body { get; set; }
        public string attachment { get; set; }
        public Mail(Partner partner, String date, int totalTrans, float totalWeight, String filePath)
        {
            subject = "(IScale) Report Import/Export Transaction of " + partner.DisplayName;
            body = "IScale would like to send you the statistics of transactions made over a period of time" + date + " on your IScale account, details are as follows:\r\n" +
                "Requester : " + partner.DisplayName + "\r\n" +
                "Account name : " + partner.Account + "\r\n" +
                "Total of transactions : " + totalTrans + "\r\n" +
                "Total weight : " + totalWeight + "\r\n" +
                "Execution time : " + date + "\r\n" +
                "Time to sent statistics : " + DateTime.Now.ToString("dd-MMMM-yyyy hh:mm") + "\r\n" +
                "Best regards and thank you,\r\n" +
                "IScale";
        }
    }
}
