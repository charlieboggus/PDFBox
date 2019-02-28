using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PDFBox.Api.Models;

namespace PDFBox.Api.Data
{
    public class DocumentContext : DbContext
    {
        public DocumentContext(DbContextOptions< DocumentContext > options) : base(options)
        {
        }

        public DbSet< Document > Documents { get; set; }
    }
}