
namespace PDFBox.Api.Models.Dtos
{
    // Data Transfer Object (DTO) that contains user data that will be transferred from the client
    // to the API
    public class UserDto
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}