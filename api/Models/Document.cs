using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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

        // <summary>
        //  Method for converting a document to PDF given a user and IFormFile
        // </summary>
        public static Document Convert(User owner, FileInfo localFile)
        {
            switch (localFile.Extension)
            {
                case ".doc":
                case ".docx":
                    ConvertWord(localFile);
                    break;
                
                case ".ppt":
                case ".pptx":
                    ConvertPPT(localFile);
                    break;
                
                case ".xls":
                case ".xlsx":
                    ConvertExcel(localFile);
                    break;
                
                default:
                    break;
            }

            localFile = new FileInfo(Path.Combine("~temp", Path.GetFileNameWithoutExtension(localFile.Name) + ".pdf"));
            using (var stream = localFile.OpenRead())
            {
                using (var br = new BinaryReader(stream))
                {
                    var filename = localFile.Name;
                    var ext = localFile.Extension;
                    var size = localFile.Length;
                    var data = br.ReadBytes((Int32) stream.Length);

                    var doc = new Document()
                    {
                        Name = filename,
                        Extension = ext,
                        Size = size,
                        CreationDate = DateTime.Now,
                        Data = data
                    };
                    if (owner != null)
                    {
                        doc.OwnerId = owner.UserId;
                        doc.Owner = owner;
                    }

                    return doc;
                }
            }
        }

        // <summary>
        //  Method to convert a given Microsoft Word document (.doc or .docx) to PDF
        //  using the Microsoft.Office.Interop.Word library
        // </summary>
        private static void ConvertWord(FileInfo file)
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
        private static void ConvertPPT(FileInfo file)
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
        private static void ConvertExcel(FileInfo file)
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