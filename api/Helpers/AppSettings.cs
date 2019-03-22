using System;

namespace PDFBox.Api.Helpers
{
    public class AppSettings
    {
        // Used to sign and verify JWT tokens
        public string JwtSecret { get; set; }

        // SMTP Credentials used for sending contact form emails
        public string SMTPUser { get; set; }

        public string SMTPPass { get; set; }
    }
}