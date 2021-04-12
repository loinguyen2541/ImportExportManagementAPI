using ImportExportManagement_API.Models;
using Microsoft.AspNetCore.Html;
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

            string htmlString = @"<html>
                                    <body>
                                        <div style=""border - style: solid;"">
                                        <div><h3><b>IScale</b> would like to send you <b>the statistics of transactions</b> made over a period of time <b>" + date + @"</b> on your IScale account, details are as follows:</h3></div>
                                        <br/>
                                        <div>Requester: <b>" + partner.DisplayName + @"</b></div>
                                        <div>Account name: <b>" + partner.Account + @"</b></div>
                                        <div>Total number of transactions: <b>" + totalTrans + @"</b></div>
                                        <div>Total weight: <b>" + totalWeight + @" KG</b></div>
                                        <div>Execution time: <b>" + date + @"</b></div>
                                        <div>Time to submit statistics: <b>" + DateTime.Now.ToString("dd/MM/yyyy hh:mm") + @"</b></div>
                                        <br/>
                                        <div>Download link: <b>" + filePath + @"</div>
                                        <div>Sincerely thank you,</div>
                                        <div><b>IScale</b></div>
                                        </div>
                                     </body>
                                  </html>";
            body = htmlString;
        }
    }
}
