using System;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PDFBox.Api.Helpers;
using PDFBox.Api.Models.Dtos;

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

        // HTTP POST: /api/contact/submit
        // <summary>
        //  API method that is called when a user submits a contact form message
        // </summary>
        [HttpPost("submit")]
        public IActionResult SubmitContactForm([FromBody] ContactDto form)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Incomplete contact form." });
            
            using (var msg = new MailMessage("contact@pdfbox.com", "webmaster@pdfbox.com"))
            {
                // Configure the email message to send
                msg.To.Add(new MailAddress("webmaster@pdfbox.com"));
                msg.From = new MailAddress("contact@pdfbox.com");
                msg.Subject = "PDFBox: New Contact Form Submission from " + form.Name;
                msg.Body = 
                    "Submission Details:\n" +
                    "-----------------------------------------------------------\n" +
                    "Name: " + form.Name + "\n" +
                    "Email: " + form.Email + "\n" +
                    "Message: \n" +
                    form.Message + "\n";
                
                // Configure SMTP client
                var client = new SmtpClient("smtp.mailtrap.io", 2525)
                {
                    Credentials = new NetworkCredential(appSettings.SMTPUser, appSettings.SMTPPass),
                    EnableSsl = true
                };

                // Send the configured email using SMTP client
                try 
                {
                    client.Send(msg);
                    return Ok(new { message = "Message successfully sent!" });
                } 
                catch (Exception e) 
                {
                    return BadRequest(new { message = "Failed to send message: " + e.Message });
                }
            }
        }
    }
}