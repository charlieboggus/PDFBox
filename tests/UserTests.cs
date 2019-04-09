using System;
using Xunit;
using PDFBox.Api.Models;

namespace tests
{
    public class UserTests
    {
        [Fact]
        public void VerifyUserPassword()
        {
            // Arrange
            var user = new User()
            {
                Username = "test",
                Email = "test@test.com"
            };

            // Act
            user.CreatePasswordHash("testPassword");
            var result = user.VerifyPasswordHash("testPassword");   // Should be true

            // Assert
            Assert.Equal(result, true);
        }

        [Fact]
        public void ChangeUserPassword()
        {
            // Arrange
            var user = new User()
            {
                Username = "test",
                Email = "test@test.com"
            };

            // Act
            user.CreatePasswordHash("testPassword");
            var verifyA = user.VerifyPasswordHash("testPassword");  // Should be true

            user.CreatePasswordHash("newPassword");
            var verifyB = user.VerifyPasswordHash("testPassword");  // Should be false
            var verifyC = user.VerifyPasswordHash("newPassword");   // Should be true

            // Assert
            Assert.Equal(verifyA, true);
            Assert.Equal(verifyB, false);
            Assert.Equal(verifyC, true);
        }
    }
}