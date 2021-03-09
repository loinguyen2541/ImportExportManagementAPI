using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
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
            /* try {
                 string captchaCode = Captcha.GenerateCaptchaCode();
                 byte[] code = Encoding.ASCII.GetBytes(captchaCode);
                 HttpContext.Session.Set("captcha", code);
                 MailMessage mail = new MailMessage();
                 SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                 mail.From = new MailAddress("tanntse63184@gmail.com");
                 mail.To.Add(mailOfManager);
                 mail.Subject = "Request Captcha";
                 mail.Body = "Your Captcha is " + captchaCode;

                 SmtpServer.Port = 587;
                 SmtpServer.Credentials = new System.Net.NetworkCredential("tanntse63184@gmail.com", "thanhtan1998");
                 SmtpServer.EnableSsl = true;
                 SmtpServer.Send(mail);
                 return Ok();
             } catch (Exception e){
                 throw new Exception();
                 return NotFound(e.Message);
            
            }*/
            string captchaCode = Captcha.GenerateCaptchaCode();
            byte[] code = Encoding.ASCII.GetBytes(captchaCode);
            HttpContext.Session.Set("captcha", code);
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("tanntse63184@fpt.edu.vn", "ConCacNe", System.Text.Encoding.UTF8);
                mail.To.Add(mailOfManager);
                mail.Subject = "Request Captcha";
                mail.Body = "Your Captcha is " + captchaCode;
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("tanntse63184@fpt.edu.vn", "thanhtan1998");
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Send(mail);
                }
            }
            return Ok();

        }
        [HttpGet("check")]
        public IActionResult CheckCaptcha(string usercaptcha)
        {
            if (HttpContext.Session.IsAvailable)
            {
                byte[] code;
                HttpContext.Session.TryGetValue("captcha", out code);
                if (usercaptcha.Length != 6)
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
                return Unauthorized();
            }
            return Unauthorized();
        }

      
    }
}
