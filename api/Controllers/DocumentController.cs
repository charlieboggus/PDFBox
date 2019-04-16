using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDFBox.Api.Data;
using PDFBox.Api.Models;

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
        public async Task< IActionResult > GetAllDocumentDetails()
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
                returnList.Add(new { doc.DocumentId, doc.Name, doc.Extension, doc.Size, creationDate = doc.CreationDate.ToShortDateString() });

            return Ok(returnList);
        }

        // HTTP GET: /api/documents/details/{ id }
        // <summary>
        //  API method for retrieving the details for a single document a user has stored
        // </summary>
        [HttpGet("details/{id}")]
        public async Task< IActionResult > GetDocumentDetails([FromRoute] int id)
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
            return Ok(new { doc.DocumentId, doc.Name, doc.Extension, doc.Size, creationDate = doc.CreationDate.ToShortDateString() });
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

            // Find the document with the requested id
            var doc = user.Documents.Where(x => x.DocumentId == id && x.OwnerId == authId).SingleOrDefault();
            if (doc == null)
                return NotFound(new { message = "The requested document could not be found." });

            // Return the file to client for download
            return File(doc.Data, GetContentType(doc.Extension), doc.Name);
        }

        // HTTP POST: /api/documents/upload
        // <summary>
        //  API method for uploading and storing a new document in database.
        // </summary>
        [HttpPost("upload")]
        public async Task< IActionResult > UploadDocuments()
        {
            // Find currently authorized user in database since you need to be logged into an account to upload documents
            var authId = Int32.Parse(HttpContext.User.Identity.Name);
            var user = await db.Users.FindAsync(authId);
            if (user == null || user.UserId != authId)
                return Unauthorized(new { message = "You are not authorized to perform that action." });

            // Get the uploaded files and convert flag from the HTTP Request Form
            var files = Request.Form.Files;
            var convert = Request.Form["convert"] == "true";

            // If the uploaded documents should be converted to PDF call the UploadConvert method,
            // otherwise call the UploadNoConvert method
            if (convert)
                return await UploadConvert(user, files);
            else
                return await UploadNoConvert(user, files);
        }

        // HTTP POST: /api/documents/convert
        // <summary>
        //  API method for converting a document to PDF without uploading it to the database. A user account is not needed for this.
        // </summary>
        [AllowAnonymous]
        [HttpPost("convert")]
        public async Task< IActionResult > ConvertDocument()
        {
            // Get the uploaded file from the request form
            var file = Request.Form.Files[0];

            // Create the temporary directory for creating files if it doesn't exist
            if (!Directory.Exists("~temp"))
                Directory.CreateDirectory("~temp");
            
            // Save the uploaded file to the temporary directory
            var filePath = Path.Combine("~temp", file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var localFile = new FileInfo(filePath);
            
            // Conver the document
            var doc = Document.Convert(null, localFile);

            // Delete the temporary directory since we don't need it anymore
            var dir = new DirectoryInfo("~temp");
            foreach (var f in dir.GetFiles())
                System.IO.File.Delete(f.FullName);
            Directory.Delete(dir.FullName);

            // Return the file to client for download
            return File(doc.Data, GetContentType(doc.Extension), doc.Name);
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

        // <summary>
        //  Method to upload documents to database and convert them to PDF.
        // </summary>
        private async Task< IActionResult > UploadConvert(User user, IFormFileCollection files)
        {
            try
            {
                // Create the temporary directory for creating files if it doesn't exist
                if (!Directory.Exists("~temp"))
                    Directory.CreateDirectory("~temp");
                
                // Save each file from the HTTP Request Form to the temporary directory
                foreach (var file in files)
                {
                    var filePath = Path.Combine("~temp", file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    var localFile = new FileInfo(filePath);

                    var doc = Document.Convert(user, localFile);
                    await db.Documents.AddAsync(doc);
                }

                // Delete the temporary directory since we don't need it anymore
                var dir = new DirectoryInfo("~temp");
                foreach (var file in dir.GetFiles())
                    System.IO.File.Delete(file.FullName);
                Directory.Delete(dir.FullName);

                // Finally save database changes
                await db.SaveChangesAsync();

                return Ok(new { message = "Successfully uploaded files." });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Unable to upload documents: " + e.Message });
            }
        }

        // <summary>
        //  Method to upload given documents to the database without converting them to PDF
        // </summary>
        private async Task< IActionResult > UploadNoConvert(User user, IFormFileCollection files)
        {
            try
            {
                foreach (var file in files)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        using (var br = new BinaryReader(stream))
                        {
                            var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            var extension = filename.Substring(filename.LastIndexOf('.')).ToLower();
                            var data = br.ReadBytes((Int32) stream.Length);
                            var size = data.Length;

                            var doc = new Document
                            {
                                Name = filename,
                                Extension = extension,
                                Size = size,
                                Data = data,
                                CreationDate = DateTime.Now,
                                OwnerId = user.UserId,
                                Owner = user
                            };

                            await db.Documents.AddAsync(doc);
                        }
                    }
                }

                await db.SaveChangesAsync();

                return Ok(new { message = "Successfully uploaded files." });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Unable to convert documents: " + e.Message });
            }
        }

        // <summary>
        //  Helper method for any method returning a download file to the client.
        //  Given a file extension, this method returns the HTML content type back to calling function.
        // </summary>
        private string GetContentType(string ext)
        {
            switch (ext)
            {
                case ".txt":        return "text/plain";
                case ".csv":        return "text/csv";
                case ".pdf":        return "application/pdf";
                case ".doc":        return "application/vnd.ms-word";
                case ".docx":       return "application/vnd.ms-word";
                case ".xls":        return "application/vnd.ms-excel";
                case ".xlsx":       return "application/vnd.openxmlformats.officedocument.spreadsheetml.sheet";
                case ".png":        return "image/png";
                case ".jpg":        return "image/jpeg";
                case ".jpeg":       return "image/jpeg";
                case ".gif":        return "image/gif";

                default:            return "text/plain";
            }
        }
    }
}