using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PDFBox.Api.Models
{
    // User model
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string  Email { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public DateTime RegistrationDate { get; set; }

        // TODO: need to implement documents
        // public List< Document > Documents { get; set; }
    }
}