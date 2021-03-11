using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/captcha")]
    [ApiController]
    public class CaptchaController : Controller
    {
        [HttpGet]
        public IActionResult GetCaptcha(string mailOfManager)
        {
            bool checkIsValidMail = true;
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(mailOfManager);
            if (!match.Success)
            {
                checkIsValidMail = false;
            }

            if (mailOfManager == null || mailOfManager.Length == 0 || !checkIsValidMail)
            {
                return NotFound();
            }
            if (HttpContext.Session.IsAvailable)
            {
                string captchaCode = Captcha.GenerateCaptchaCode();
                byte[] code = Encoding.ASCII.GetBytes(captchaCode);
                HttpContext.Session.Set("captcha", code);
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("tanntse63184@fpt.edu.vn", "ICAN Automatic Mailer ", System.Text.Encoding.UTF8);
                    mail.To.Add(mailOfManager);
                    mail.Subject = "Request Captcha";
                    mail.Body = "<h1>Your Captcha is " + captchaCode +"</h1>";
                    mail.IsBodyHtml = true;
                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential("tanntse63184@fpt.edu.vn", "thanhtan1998");
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Send(mail);
                    }
                }
                return Ok(captchaCode);
            }
            return Unauthorized();

        }
        [HttpGet("check")]
        public IActionResult CheckCaptcha([FromQuery] string usercaptcha)
        {
            if (HttpContext.Session.IsAvailable)
            {
                byte[] code;
                HttpContext.Session.TryGetValue("captcha", out code);
                if (usercaptcha == null || usercaptcha.Length != 6)
                {
                    return Unauthorized();
                }
                if (code != null)
                {
                    string captcha = Encoding.ASCII.GetString(code);
                    if (captcha.Equals(usercaptcha))
                    {
                        return Ok();
                    }
                }
                else
                {
                    return NotFound();
                }

            }
            return Unauthorized();
        }


    }
}
