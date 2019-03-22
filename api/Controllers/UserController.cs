using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PDFBox.Api.Data;
using PDFBox.Api.Models;
using PDFBox.Api.Helpers;

namespace PDFBox.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly PDFBoxContext db;

        private readonly AppSettings appSettings;

        public UserController(PDFBoxContext db, IOptions< AppSettings > appSettings)
        {
            this.db = db;
            this.appSettings = appSettings.Value;
        }

        // Authentication method to authenticate user login credentials
        // HTTP POST: /api/users/auth
        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task< IActionResult > Authenticate([FromBody] UserDto userData)
        {
            // Verify user login credentials
            if(string.IsNullOrEmpty(userData.Username) || string.IsNullOrEmpty(userData.Password))
                return BadRequest(new { message = "Username or password is incorrect" });
            
            var user = await db.Users.SingleOrDefaultAsync(x => x.Username == userData.Username);
            if(user == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            
            if(!VerifyPasswordHash(userData.Password, user.PasswordHash, user.PasswordSalt))
                return BadRequest(new { message = "Username or password is incorrect" });

            // If user was successfully verified create a new JWT authentication token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(appSettings.JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Return basic user info (w/o password) and authentication token to store client-side
            return Ok(new 
            { 
                id = user.Id, 
                username = user.Username,
                email = user.Email,
                Token = tokenString
            });
        }

        // Registration method to register a new user
        // HTTP POST: /api/users/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task< IActionResult > Register([FromBody] UserDto userData)
        {
            // User data Validation
            if (string.IsNullOrWhiteSpace(userData.Username))
                return BadRequest(new { message = "Username is required" });

            if (string.IsNullOrWhiteSpace(userData.Email))
                return BadRequest(new { message = "Email is required" });

            if (string.IsNullOrWhiteSpace(userData.Password))
                return BadRequest(new { message = "Password is required" });

            // Existing credentials validation
            var usernameTaken = await db.Users.AnyAsync(x => x.Username == userData.Username);
            if (usernameTaken)
                return BadRequest(new { message = "Username \"" + userData.Username + "\" is already taken" });

            var emailTaken = await db.Users.AnyAsync(x => x.Email == userData.Email);
            if (emailTaken)
                return BadRequest(new { message = "Email \"" + userData.Email + "\" is already in use" });

            // After validation we can create the new User
            byte[] passwordHash;
            byte[] passwordSalt;
            CreatePasswordHash(userData.Password, out passwordHash, out passwordSalt);
            var user = new User
            {
                Username = userData.Username,
                Email = userData.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RegistrationDate = DateTime.Now
            };

            // Add the newly created user to the database & save the database changes
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            // Return HTTP Ok upon successful user registration
            return Ok(new { message = "Registration successful" });
        }

        // Returns all users registered in system
        // HTTP GET: /api/users
        [HttpGet]
        public IActionResult GetAll()
        {
            // Get all the users in the database
            IEnumerable< User > users = db.Users;

            // Create a new list  of user data to return to the client
            List< UserDto > userData = new List< UserDto >();
            foreach (User u in users)
            {
                // Create a UserDto object for each user and add it to the return list
                UserDto data = new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email
                };

                userData.Add(data);
            }

            // Return the UserDto list of all the registered users
            return Ok(userData);
        }

        // Returns user with given ID
        // HTTP GET: /api/users/{ id }
        [HttpGet("{id}")]
        public async Task< IActionResult > GetUser([FromRoute] int id)
        {
            // Try to find a user with given Id in database
            var user = await db.Users.FindAsync(id);
            if(user == null)
                return BadRequest(new { message = "User not found" });
            
            // If a user was found create a new UserDto object to return to client
            var userData = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            // Return the user data to client with HTTP Ok
            return Ok(userData);
        }

        // Method to update a user's data
        // HTTP PUT: /api/users/{ id }
        [HttpPut("{id}")]
        public async Task< IActionResult > UpdateUser([FromRoute] int id, [FromBody] UserDto newData)
        {
            var user = await db.Users.FindAsync(id);
            if (user == null)
                return BadRequest(new { message = "User not found" });

            // Username update (if it was entered)
            if (!string.IsNullOrWhiteSpace(newData.Username) && (newData.Username != user.Username))
            {
                // Check if new username is taken
                var usernameTaken = await db.Users.AnyAsync(x => x.Username == newData.Username);
                if (usernameTaken)
                    return BadRequest(new { message = "Username \"" + newData.Username + "\" is already taken" });
                
                // If it's not update it
                user.Username = newData.Username;
            }

            // Email update (if it was entered)
            if (!string.IsNullOrWhiteSpace(newData.Email) && (newData.Email != user.Email))
            {
                // Check if new email is taken
                var emailTaken = await db.Users.AnyAsync(x => x.Email == newData.Email);
                if (emailTaken)
                    return BadRequest(new { message = "Email \"" + newData.Email + "\" is already in use" });

                // If it's not update it
                user.Email = newData.Email;
            }

            // Password Update (if it was entered)
            if(!string.IsNullOrWhiteSpace(newData.Password))
            {
                // Create hash & salt for new password
                byte[] passwordHash;
                byte[] passwordSalt;
                CreatePasswordHash(newData.Password, out passwordHash, out passwordSalt);

                // Update the password hash & salt for user
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            db.Users.Update(user);
            await db.SaveChangesAsync();

            return Ok(new { message = "Successfully updated user profile" });
        }

        // Method to delete user from system
        // HTTP DELETE: /api/users/{ id }
        [HttpDelete("{id}")]
        public async Task< IActionResult > DeleteUser([FromRoute] int id)
        {
            var user = await db.Users.FindAsync(id);
            if (user == null)
                return BadRequest(new { message = "User not found" });
            
            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return Ok(new { message = "User successfully deleted" });
        }

        // Helper method to generate Hash and Salt for a password (so we don't store plaintext passwords in database)
        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            if(password == null || string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException("Password cannot be null or empty");
            
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        // Helper method to verify a given password. Returns true if given password's hash & salt match the stored hash & salt
        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            // Argument validation
            if(password == null || string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException("Password cannot be null or empty");
            if(storedHash.Length != 64)
                throw new ArgumentException("Invalid password hash length (64 bytes expected)");
            if(storedSalt.Length != 128)
                throw new ArgumentException("Invalid password salt length (128 bytes expected)");

            // Verify the password hash using the stored salt
            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                        return false;
                }
            }

            return true;
        }
    }
}