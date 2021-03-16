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
        CaptchaRepository repo;
        public CaptchaController()
        {
            repo = new CaptchaRepository();
        }

        // provide random captcha
        [HttpGet]
        public IActionResult GetCaptcha(string mailOfManager)
        {    
            if ( !repo.checkFormatMail(mailOfManager))
            {
                return NotFound();
            }
            if (HttpContext.Session.IsAvailable)
            {
                string captchaCode = repo.GenerateCaptchaCode();
                byte[] code = Encoding.ASCII.GetBytes(captchaCode);
                HttpContext.Session.Set("captcha", code);
                bool isSendMail = repo.sendMail(mailOfManager, captchaCode);
                if(isSendMail)
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
