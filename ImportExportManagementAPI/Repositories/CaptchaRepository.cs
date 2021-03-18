using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Repositories
{
    public class CaptchaRepository
    {
        const string Letters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public CaptchaRepository()
        {
        }
        public  string GenerateCaptchaCode()
        {
            Random rand = new Random();
            int maxRand = Letters.Length - 1;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i <= 5; i++)
            {
                int index = rand.Next(maxRand);
                sb.Append(Letters[index]);
            }

            return sb.ToString();
        } //generate captcha code base on alphabet
        public bool checkFormatMail(string mailOfManager)
        {
            bool checkIsValidMail = true;
            if (mailOfManager == null || mailOfManager.Length == 0) return false;
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(mailOfManager);
            if (!match.Success)
            {
                checkIsValidMail = false;
            }
            return checkIsValidMail;
        }//check format is mail is valid
        public bool sendMail(string mailOfManager,string captcha)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("tanntse63184@fpt.edu.vn", "ICAN Automatic Mailer ", System.Text.Encoding.UTF8);
                    mail.To.Add(mailOfManager);
                    mail.Subject = "Request Captcha";
                    mail.Body = "<h1>Your Captcha is " + captcha + "</h1>" +
                        "<h2>This Captcha will expire within 5 minutes</h2>";
                    mail.IsBodyHtml = true;
                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential("tanntse63184@fpt.edu.vn", "thanhtan1998");
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Send(mail);
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        //send mail service
    }
}
