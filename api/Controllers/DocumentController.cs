using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDFBox.Api.Data;
using PDFBox.Api.Models;
using System.Net.Http;
using System.Net;

namespace PDFBox.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/documents")]
    public class DocumentController : ControllerBase
    {
        private readonly PDFBoxContext db;

        public DocumentController(PDFBoxContext db)
        {
            this.db = db;
        }

        // HTTP GET: /api/documents/details/all
        // <summary>
        //  API method for retrieving the details for all documents a user has stored
        // </summary>
        [HttpGet("details/all")]
        public async Task< IActionResult > GetDocuments()
        {
            // Find the currently authorized user in the database. 
            // We're using eager loading to load all of the user's documents into the List< Document > attribute when we find the user
            var authId = Int32.Parse(HttpContext.User.Identity.Name);
            var user = await db.Users.Include("Documents").Where(x => x.UserId == authId).SingleOrDefaultAsync();
            if (user == null)
                return Unauthorized(new { message = "You are not authorized to perform that action." });

            // Create a new list to store the details of each document
            var returnList = new List< Object >();
            foreach (Document doc in user.Documents)
                returnList.Add(new { doc.DocumentId, doc.Name, doc.Extension, doc.Size, doc.CreationDate });

            return Ok(returnList);
        }

        // HTTP GET: /api/documents/details/{ id }
        // <summary>
        //  API method for retrieving the details for a single document a user has stored
        // </summary>
        [HttpGet("details/{id}")]
        public async Task< IActionResult > GetDocument([FromRoute] int id)
        {
            // Find the currently authorized user in the database. 
            // We're using eager loading to load all of the user's documents into the List< Document > attribute when we find the user.
            var authId = Int32.Parse(HttpContext.User.Identity.Name);
            var user = await db.Users.Include("Documents").Where(x => x.UserId == authId).SingleOrDefaultAsync();
            if (user == null)
                return Unauthorized(new { message = "You are not authorized to perform that action." });
            
            // Find the document with the provided id
            var doc = user.Documents.Where(x => x.DocumentId == id).SingleOrDefault();
            if (doc == null)
                return NotFound(new { message = "The requested document could not be found." });

            // If the requested document was found we can return a new JSON object with the document details
            return Ok(new { doc.DocumentId, doc.Name, doc.Extension, doc.Size, doc.CreationDate });
        }

        // HTTP GET: /api/documents/{ id }
        // <summary>
        //  API method for downloading a stored document
        // </summary>
        [HttpGet("{id}")]
        public async Task< IActionResult > DownloadDocument([FromRoute] int id)
        {
            // Find the currently authorized user in the database. 
            // We're using eager loading to load all of the user's documents into the List< Document > attribute when we find the user
            var authId = Int32.Parse(HttpContext.User.Identity.Name);
            var user = await db.Users.Include("Documents").Where(x => x.UserId == authId).SingleOrDefaultAsync();
            if (user == null)
                return Unauthorized(new { message = "You are not authorized to perform that action." });
            
            // TODO: figure this out

            return Ok();
        }

        // HTTP POST: /api/documents/convert
        // <summary>
        //  API method for converting a document to PDF without uploading it to the database
        // </summary>
        [HttpPost("convert")]
        public async Task< IActionResult > ConvertDocument()
        {
            // TODO: figure this out

            return Ok();
        }

        // HTTP POST: /api/documents/upload
        // <summary>
        //  API method for uploading and storing a new document in database
        // </summary>
        [HttpPost("upload")]
        public async Task< IActionResult > UploadDocument()
        {
            // Find currently authorized user in database
            var authId = Int32.Parse(HttpContext.User.Identity.Name);
            var user = await db.Users.FindAsync(authId);
            if (user == null || user.UserId != authId)
                return Unauthorized(new { message = "You are not authorized to perform that action." });

            try
            {
                // Get the uploaded files from the request form (can be multiple)
                var files = Request.Form.Files;

                // Iterate over each uploaded file
                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    
                    // Open a read stream for the current file
                    using (var fs = file.OpenReadStream())
                    {
                        // Open a binary reader for the current file so we can read its raw bytes
                        using (var br = new BinaryReader(fs))
                        {
                            // Read info & data from current file
                            byte[] data = br.ReadBytes((Int32) fs.Length);
                            var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            var filesize = ContentDispositionHeaderValue.Parse(file.ContentDisposition).Size;
                            var extension = filename.Substring(filename.LastIndexOf('.')).ToLower();
                            
                            // TODO: convert the document to PDF before storing it in database -- that's our whole schtick...

                            // Create a new Document object from file data
                            var doc = new Document
                            {
                                Name = filename,
                                Extension = extension,
                                Size = (filesize != null && filesize.HasValue) ? filesize.Value : data.Length,
                                CreationDate = DateTime.Now,
                                Data = data,
                                OwnerId = user.UserId,
                                Owner = user
                            };

                            // Add the newly created document to the database
                            await db.Documents.AddAsync(doc);
                        }
                    }
                }

                // Once we're done iterating over all the uploaded documents we can save the database changes & return HTTP Ok
                await db.SaveChangesAsync();

                return Ok(new { message = "Successfully uploaded files." });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Document upload failed: " + e.Message });
            }
        }

        // HTTP DELETE: /api/documents/all
        // <summary>
        //  API method for deleting all documents a user has stored
        // </summary>
        [HttpDelete("all")]
        public async Task< IActionResult > DeleteAllDocuments()
        {
            // Find the currently authorized user in the database. 
            // We're using eager loading to load all of the user's documents into the List< Document > attribute when we find the user
            var authId = Int32.Parse(HttpContext.User.Identity.Name);
            var user = await db.Users.Include("Documents").Where(x => x.UserId == authId).SingleOrDefaultAsync();
            if (user == null)
                return Unauthorized(new { message = "You are not authorized to perform that action." });
            
            // Find all the documents owned by the currently authorized user & delete them from database
            var docs = await db.Documents.Where(x => x.OwnerId == user.UserId).ToListAsync();
            if (docs == null)
                return NotFound(new { message = "Requested documents not found." });
            db.Documents.RemoveRange(docs);
            await db.SaveChangesAsync();

            return Ok(new { message = "All documents successfully deleted." });
        }

        // HTTP DELETE: /api/documents/{ id }
        // <summary>
        //  API method for deleting a single document a user has stored
        // </summary>
        [HttpDelete("{id}")]
        public async Task< IActionResult > DeleteDocument([FromRoute] int id)
        {
            // Find the currently authorized user in the database. 
            // We're using eager loading to load all of the user's documents into the List< Document > attribute when we find the user
            var authId = Int32.Parse(HttpContext.User.Identity.Name);
            var user = await db.Users.Include("Documents").Where(x => x.UserId == authId).SingleOrDefaultAsync();
            if (user == null)
                return Unauthorized(new { message = "You are not authorized to perform that action." });

            // Find the document in the database with authenticated user OwnerId and given DocumentId & remove it from DB
            var doc = await db.Documents.Where(x => x.OwnerId == user.UserId && x.DocumentId == id).SingleOrDefaultAsync();
            if (doc == null)
                return NotFound(new { message = "Requested document not found." });
            db.Documents.Remove(doc);
            await db.SaveChangesAsync();

            return Ok(new { message = "Document successfully deleted." });
        }
    }
}