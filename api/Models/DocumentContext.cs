using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PDFBox.Api.Models
{
    public class DocumentContext : DbContext
    {
        public DocumentContext(DbContextOptions< DocumentContext > options) : base(options)
        {
        }

        public DbSet< Document > Documents { get; set; }
    }
}
