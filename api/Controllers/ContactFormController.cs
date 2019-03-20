using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PDFBox.Api.Models;

namespace PDFBox.Api.Controllers
{
    [Route("api/contact")]
    [ApiController]
    public class ContactFormController : ControllerBase
    {
        [HttpPost]
        public IActionResult SubmitContactForm([FromBody] ContactForm form)
        {
            if(!ModelState.IsValid)
            {
                return Ok(new { success = false });
            }

            var result = this.SendMail(form);

            return Ok(new { success = result });
        }

        private bool SendMail(ContactForm form)
        {
            using(var message = new MailMessage(form.Email, "webmaster@pdfbox.com"))
            {
                message.To.Add(new MailAddress("webmaster@pdfbox.com"));
                message.From = new MailAddress(form.Email);
                message.Subject = "PDFBox: New Contact Form Submission from " + form.Name;

                // Format the mail body
                string mailBody = 
                    "New contact form submission from PDFBox:\n\n" + 
                    "Name: " + form.Name + "\n" +
                    "Email: " + form.Email + "\n\n" +
                    "Message: \n" +
                    form.Message + "\n";

                message.Body = mailBody;

                var client = new SmtpClient("smtp.mailtrap.io", 2525)                       // TODO: get a real smtp client maybe?
                {
                    Credentials = new NetworkCredential("60de777dc8ec71", "2bc0fe69516b17"),
                    EnableSsl = true
                };

                try {
                    client.Send(message);
                    return true;
                }
                catch (Exception) {
                    return false;
                }
            }
        }
    }
}