using Microsoft.EntityFrameworkCore;
using PDFBox.Api.Models;

namespace PDFBox.Api.Data
{
    public class PDFBoxContext : DbContext
    {
        public DbSet< User > Users { get; set; }

        public DbSet< Document > Documents { get; set; }

        public PDFBoxContext(DbContextOptions< PDFBoxContext > options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder) => base.OnModelCreating(builder);
    }
}