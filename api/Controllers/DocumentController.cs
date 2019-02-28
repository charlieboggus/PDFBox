using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PDFBox.Api.Data;
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

        // GET: api/documents/
        [HttpGet]
        public IEnumerable< Document > Get()
        {
            return db.Documents;
        }

        // GET: api/documents/{id}
        [HttpGet("{id}", Name = "GetDocument")]
        public async Task< IActionResult > GetDocument([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var doc = await db.Documents.SingleOrDefaultAsync(d => d.Id == id);
            if(doc == null)
                return NotFound();
            
            return Ok(doc);
        }

        // PUT: api/documents/{id}
        [HttpPut("{id}")]
        public async Task< IActionResult > Put([FromRoute] int id, [FromBody] Document doc)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if(id != doc.Id)
                return BadRequest();
            
            db.Entry(doc).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!DocumentExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/documents/
        [HttpPost]
        public async Task< IActionResult > Post([FromBody] Document doc)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            db.Documents.Add(doc);
            await db.SaveChangesAsync();

            return CreatedAtAction("GetDocument", new { id = doc.Id}, doc);
        }

        // DELETE: api/documents/{id}
        [HttpDelete("{id}")]
        public async Task< IActionResult > Delete([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var doc = await db.Documents.SingleOrDefaultAsync(d => d.Id == id);
            if(doc == null)
                return NotFound();
            
            db.Documents.Remove(doc);
            await db.SaveChangesAsync();

            return Ok(doc);
        }

        private bool DocumentExists(int id)
        {
            return db.Documents.Any(e => e.Id == id);
        }
    }
}