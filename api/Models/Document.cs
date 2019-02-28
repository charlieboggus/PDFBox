using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDFBox.Api.Models
{
    public class Document
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Size { get; set; }

        public DateTime DateCreated { get; set; }

        public byte[] Data { get; set; }
    }
}
