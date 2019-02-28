using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PDFBox.Api.Models;

namespace PDFBox.Api.Controllers
{
    [Route("api/documents")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentContext db;

        public DocumentController(DocumentContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(db.Documents);
        }

        [HttpGet("{id}", Name = "GetDocument")]
        public IActionResult GetDocument(int id)
        {
            // Try to find the document in the DB with the given id
            var doc = this.db.Documents.FirstOrDefault(d => d.Id == id);

            // If it doesn't exist return HTTP 404 Not Found
            if(doc == null)
                return NotFound();

            // Return the Document inside HTTP 200 OK
            return Ok(doc);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Document doc)
        {
            if (doc == null)
                return BadRequest();

            this.db.Documents.Add(doc);
            this.db.SaveChanges();

            return CreatedAtRoute("GetDocument", new { id = doc.Id }, doc);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Document doc)
        {
            if (doc == null || doc.Id != id)
                return BadRequest();

            var docToEdit = this.db.Documents.FirstOrDefault(d => d.Id == id);
            if (docToEdit == null)
                return NotFound();

            docToEdit.Name = doc.Name;
            docToEdit.Size = doc.Size;
            docToEdit.DateCreated = doc.DateCreated;
            docToEdit.Data = doc.Data;

            this.db.Documents.Update(docToEdit);
            this.db.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var doc = this.db.Documents.FirstOrDefault(d => d.Id == id);
            if (doc == null)
                return NotFound();

            this.db.Documents.Remove(doc);
            this.db.SaveChanges();

            return NoContent();
        }
    }
}
