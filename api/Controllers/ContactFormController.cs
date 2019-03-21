using System;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using PDFBox.Api.Models;

namespace PDFBox.Api.Controllers
{
    [Route("api/contact")]
    [ApiController]
    public class ContactFormController : ControllerBase
    {
        [HttpPost]
        public IActionResult SubmitForm([FromBody] ContactForm form)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new { success = false });
            }

            using(var message = new MailMessage("contact@pdfbox.com", "webmaster@pdfbox.com"))
            {
                // Configure the message to send
                message.To.Add(new MailAddress("webmaster@pdfbox.com"));
                message.From = new MailAddress("contact@pdfbox.com");
                message.Subject = "PDFBox: New Contact Form Submission from " + form.Name;
                string mailBody = 
                    "Submission Details:\n" + 
                    "-----------------------------------------------------------\n" +
                    "Name: " + form.Name + "\n" +
                    "Email: " + form.Email + "\n" +
                    "Message: \n" +
                    "\t" + form.Message + "\n";
                message.Body = mailBody;

                // Configure SMTP client
                var client = new SmtpClient("smtp.mailtrap.io", 2525)                       // TODO: get a real smtp client maybe?
                {
                    Credentials = new NetworkCredential("60de777dc8ec71", "2bc0fe69516b17"),
                    EnableSsl = true
                };

                try {
                    // Send the email
                    client.Send(message);

                    // Then return HTTP Ok with success = true
                    return Ok(new { success = true });
                }
                catch (Exception e) {
                    // If something goes wrong return HTTP Ok with success = false and the error message
                    return Ok(new { success = false, error = e.Message });
                }
            }
        }
    }
}