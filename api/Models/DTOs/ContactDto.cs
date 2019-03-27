using System.ComponentModel.DataAnnotations;

namespace PDFBox.Api.Models.Dtos
{
    // Data Transfer Object (DTO) that contains contact form data that will be transferred from the client
    // to the API
    public class ContactDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Message { get; set; }
    }
}