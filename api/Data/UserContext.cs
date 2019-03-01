using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PDFBox.Api.Models;

namespace PDFBox.Api.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions< UserContext > options) : base(options)
        {
        }

        public DbSet< User > Users { get; set; }
    }
}