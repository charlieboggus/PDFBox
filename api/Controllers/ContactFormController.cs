using System;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PDFBox.Api.Helpers;
using PDFBox.Api.Models;

namespace PDFBox.Api.Controllers
{
    [ApiController]
    [Route("api/contact")]
    public class ContactFormController : ControllerBase
    {
        private readonly AppSettings appSettings;

        public ContactFormController(IOptions< AppSettings > appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        [HttpPost]
        public IActionResult SubmitForm([FromBody] ContactFormDto data)
        {
            // Validate the Model State
            if(!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid Model State" });
            }

            using(var message = new MailMessage("contact@pdfbox.com", "webmaster@pdfbox.com"))
            {
                // Configure the message to send
                message.To.Add(new MailAddress("webmaster@pdfbox.com"));
                message.From = new MailAddress("contact@pdfbox.com");
                message.Subject = "PDFBox: New Contact Form Submission from " + data.Name;
                string mailBody = 
                    "Submission Details:\n" + 
                    "-----------------------------------------------------------\n" +
                    "Name: " + data.Name + "\n" +
                    "Email: " + data.Email + "\n" +
                    "Message: \n" +
                    "\t" + data.Message + "\n";
                message.Body = mailBody;

                // Configure SMTP client
                // TODO: get a real smtp client maybe? We'd need to own pdfbox.com though
                var client = new SmtpClient("smtp.mailtrap.io", 2525)
                {
                    Credentials = new NetworkCredential(appSettings.SMTPUser, appSettings.SMTPPass),
                    EnableSsl = true
                };

                // Try and send the email
                try {
                    client.Send(message);
                    return Ok(new { success = true, message = "Message successfully sent" });
                }
                catch (Exception e) {
                    return Ok(new { success = false, message = e.Message });
                }
            }
        }
    }
}