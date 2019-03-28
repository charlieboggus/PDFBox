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
using Microsoft.AspNetCore.Http;

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
                returnList.Add(new { doc.DocumentId, doc.Name, doc.Extension, doc.Size, doc.CreationDate });

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
            // TODO: figure this out

            return Ok();
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
                    var filepath = Path.Combine("~temp", file.FileName);
                    using (var stream = new FileStream(filepath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                // Get all the files in the temporary directory & iterate over each of them to try and convert them to PDF
                var dir = new DirectoryInfo("~temp");
                var localFiles = dir.GetFiles();
                foreach (var file in localFiles)
                {
                    // If the file extension is a Word extension use the convert Word method
                    if (file.Extension == ".doc" || file.Extension == ".docx")
                        ConvertWordToPdf(file);

                    // If the file extension is a PPT extension use the convert PPT method
                    if (file.Extension == ".ppt" || file.Extension == ".pptx")
                        ConvertPowerPointToPdf(file);
                    
                    // If the file extension is an Excel extension use the convert Excel method
                    if (file.Extension == ".xls" || file.Extension == ".xlsx")
                        ConvertExcelToPdf(file);

                    // No other file types are supported for conversion to PDF at this time...
                }

                // Refresh the directory and get all the files in it again so we can store them in database
                dir.Refresh();
                localFiles = dir.GetFiles();
                foreach (var file in localFiles)
                {
                    using (var stream = file.OpenRead())
                    {
                        using (var br = new BinaryReader(stream))
                        {
                            // Get file info
                            var filename = file.Name;
                            var extension = file.Extension;
                            var size = file.Length;
                            var data = br.ReadBytes((Int32) stream.Length);

                            // Create a new Document object from file info
                            var doc = new Document
                            {
                                Name = filename,
                                Extension = extension,
                                Size = size,
                                CreationDate = DateTime.Now,
                                Data = data,
                                OwnerId = user.UserId,
                                Owner = user
                            };

                            // Store the new document object in database
                            await db.Documents.AddAsync(doc);
                        }
                    }
                }

                // Delete the temporary directory since we don't need it anymore
                foreach (var file in localFiles)
                {
                    System.IO.File.Delete(file.FullName);
                }
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

        // <summary>
        //  Method to convert a given Microsoft Word document (.doc or .docx) to PDF
        //  using the Microsoft.Office.Interop.Word library
        // </summary>
        private void ConvertWordToPdf(FileInfo file)
        {
            object missing = System.Reflection.Missing.Value;

            // Initialize the Interop Word Applicaiton for conversion
            var word = new Microsoft.Office.Interop.Word.Application();
            word.Visible = false;
            word.ScreenUpdating = false;
            word.DisplayAlerts = Microsoft.Office.Interop.Word.WdAlertLevel.wdAlertsNone;

            // Open the word document file
            Object filename = file.FullName;
            var doc = word.Documents.Open(
                ref filename, ref missing, ref missing, ref missing, 
                ref missing, ref missing, ref missing, ref missing, 
                ref missing, ref missing, ref missing, ref missing, 
                ref missing, ref missing, ref missing, ref missing
            );
            doc.Activate();

            // Save it as PDF
            object outfile = file.FullName.Replace(file.Extension, ".pdf");
            object format = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF;
            doc.SaveAs2(
                ref outfile, ref format, ref missing, ref missing, 
                ref missing, ref missing, ref missing, ref missing, 
                ref missing, ref missing, ref missing, ref missing, 
                ref missing, ref missing, ref missing, ref missing
            );

            // Close the document and word application
            object saveChanges = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
            ((Microsoft.Office.Interop.Word._Document) doc).Close(ref saveChanges, ref missing, ref missing);
            doc = null;

            ((Microsoft.Office.Interop.Word._Application) word).Quit(ref missing, ref missing, ref missing);
            word = null;

            // Delete the original file since it's not needed anymore after conversion
            System.IO.File.Delete(file.FullName);
        }

        // <summary>
        //  Method to convert a given Microsoft PowerPoint presentation (.ppt or .pptx) to PDF
        //  using the Microsoft.Office.Interop.PowerPoint library
        // </summary>
        private void ConvertPowerPointToPdf(FileInfo file)
        {
            // Initialize Interop PowerPoint Application
            var app = new Microsoft.Office.Interop.PowerPoint.Application();

            // Open the PPT file
            var ppt = app.Presentations.Open(file.FullName, Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoTriState.msoFalse);
            
            // Export the file as PDF
            var outfile = file.FullName.Replace(file.Extension, ".pdf");
            ppt.SaveAs(outfile, Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType.ppSaveAsPDF, Microsoft.Office.Core.MsoTriState.msoTrue);

            // Close everything
            ppt.Close();
            ppt = null;

            app.Quit();
            app = null;

            // Delete the original file since it's not needed anymore after conversion
            System.IO.File.Delete(file.FullName);
        }

        // <summary>
        //  Method to convert a given Microsoft Excel workbook to PDF
        //  using the Microsoft.Office.Interop.Excel library
        // </summary>
        private void ConvertExcelToPdf(FileInfo file)
        {
            object missing = System.Reflection.Missing.Value;

            // Initialize Interop Excel Application
            var app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = false;
            app.ScreenUpdating = false;
            app.DisplayAlerts = false;

            // Open the Excel workbook
            var xlsx = app.Workbooks.Open(
                file.FullName, missing, missing, missing, 
                missing, missing, missing, missing, 
                missing, missing, missing, missing, 
                missing, missing, missing
            );

            // Export the workbook as PDF
            var outfile = file.FullName.Replace(file.Extension,  ".pdf");
            xlsx.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, outfile, missing, missing, missing, missing, missing, missing, missing);

            // Close everything
            xlsx.Close(Microsoft.Office.Interop.Excel.XlSaveAction.xlDoNotSaveChanges, missing, missing);
            xlsx = null;

            app.Quit();
            app = null;

            // Delete the original file since it's not needed anymore after conversion
            System.IO.File.Delete(file.FullName);
        }
    }
}