using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PDFBox.Api.Models
{
    // Document model
    public class Document
    {
        public int OwnerId { get; set; }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Size { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        // TODO: Document data -- byte[] maybe? figure out how to store PDF docs...
    }
}