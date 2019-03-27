using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PDFBox.Api.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public DateTime RegistrationDate { get; set; }

        public virtual List< Document > Documents { get; set; }
    }
}