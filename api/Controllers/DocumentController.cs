using System;
using System.Linq;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PDFBox.Api.Data;
using PDFBox.Api.Helpers;
using PDFBox.Api.Models;
using PDFBox.Api.Models.Dtos;

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
                return BadRequest(new { message = "Requested document not found." });

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
        //  API method for converting a document to PDF
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
            
            // TODO: figure out this

            return Ok();
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

            // TODO: figure this out

            return Ok();
        }
    }
}