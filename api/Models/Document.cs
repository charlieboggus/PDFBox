using System;

namespace PDFBox.Api.Models
{
    public class Document
    {
        // --------------- Document Information ---------------

        public int DocumentId { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public long Size { get; set; }

        public DateTime CreationDate { get; set; }

        public byte[] Data { get; set; }

        // --------------- Owner Information ---------------
        
        public int OwnerId { get; set; }

        public User Owner { get; set; }
    }
}