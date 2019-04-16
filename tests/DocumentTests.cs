using System;
using System.IO;
using Xunit;
using PDFBox.Api.Models;

namespace tests
{
    public class DocumentTests
    {
        [Fact]
        public void VerifyWordConversion()
        {
            // Arrange
            var file = new FileInfo(Path.Combine("resources", "test_doc.docx"));

            // Act
            var doc = Document.Convert(null, file);

            // Assert
            Assert.Equal(doc.Extension, ".pdf");
            Assert.Equal(doc.Name, "test_doc.pdf");
        }

        [Fact]
        public void VerifyPPTConversion()
        {
            // Arrange
            var file = new FileInfo(Path.Combine("resources", "test_ppt.pptx"));

            // Act
            var doc = Document.Convert(null, file);

            // Assert
            Assert.Equal(doc.Extension, ".pdf");
            Assert.Equal(doc.Name, "test_ppt.pdf");
        }

        [Fact]
        public void VerifyExcelConversion()
        {
            // Arrange
            var file = new FileInfo(Path.Combine("resources", "test_ppt.xlsx"));

            // Act
            var doc = Document.Convert(null, file);

            // Assert
            Assert.Equal(doc.Extension, ".pdf");
            Assert.Equal(doc.Name, "test_xlsx.pdf");
        }
    }
}