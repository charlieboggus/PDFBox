using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PDFBox.Api.Models
{
    // Model for a Login attempt
    public class LoginModel
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}