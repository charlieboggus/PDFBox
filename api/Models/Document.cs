using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PDFBox.Api.Models
{
    public class Document
    {

        [Required]
        public int OwnerId { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public int Size { get; set; }

        public DateTime DateCreated { get; set; }

        // TODO: Document data -- byte[] maybe? figure out how to store PDF docs...
    }
}