using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

        // HTTP POST: /api/users/register
        // <summary>
        //  API method to register a new user.
        // </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task< IActionResult > RegisterUser([FromBody] UserDto userData)
        {
            // Argument verification
            if (string.IsNullOrWhiteSpace(userData.Username))
                return BadRequest(new { message = "Username is required!" });
            if (string.IsNullOrWhiteSpace(userData.Email))
                return BadRequest(new { message = "Email is required!" });
            if (string.IsNullOrWhiteSpace(userData.Password))
                return BadRequest(new { message = "Password is required!" });
            
            // Check if provided username is already taken
            var usernameTaken = await db.Users.AnyAsync(x => x.Username == userData.Username);
            if (usernameTaken)
                return BadRequest(new { message = "Username \"" + userData.Username + "\" is unavailable." });

            // Check if provided email address is already in use
            var emailTaken = await db.Users.AnyAsync(x => x.Email == userData.Email);
            if (emailTaken)
                return BadRequest(new { message = "Email \"" + userData.Email + "\" is already in use." });

            // If username and email are available we can create a new user
            var user = new User
            {
                Username = userData.Username,
                Email = userData.Email,
                RegistrationDate = DateTime.Now
            };
            user.CreatePasswordHash(userData.Password);

            // Add newly created user to the DB and save DB changes
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            return Ok(new { message = "User registration successful!" });
        }

        // HTTP POST: /api/users/authenticate
        // <summary>
        //  API method to authenticate a user. Used when a user logs in.
        // </summary>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task< IActionResult > AuthenticateUser([FromBody] UserDto userData)
        {
            // Argument verification
            if (string.IsNullOrWhiteSpace(userData.Username) || string.IsNullOrWhiteSpace(userData.Password))
                return BadRequest(new { message = "Invalid username or password." });
            
            // Find the user with the provided username in the database
            var user = await db.Users.SingleOrDefaultAsync(x => x.Username == userData.Username);
            if (user == null)
                return BadRequest(new { message = "Invalid username or password." });
            
            // Verify the hash of the provided password with the stored hash/salt of the user
            if (!user.VerifyPasswordHash(userData.Password))
                return BadRequest(new { message = "Invalid username or password." });
            
            // If user was successfully verified we need to create a new JWT authentication token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(appSettings.JwtSecret);
            var tokenDesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDesc);
            var tokenString = tokenHandler.WriteToken(token);

            // Return authentication details back to client
            return Ok(new { id = user.UserId, user.Username, Token = tokenString });
        }

        // HTTP GET: /api/users/all
        // <summary>
        //  API method to get information about every registered user
        // </summary>
        [AllowAnonymous]
        [HttpGet("all")]
        public async Task< IActionResult > GetAllUsers()
        {
            // TODO: might delete this method as it may be a security risk...

            var users = await db.Users.ToListAsync();
            var returnList = new List< Object >();
            foreach (User u in users)
                returnList.Add(new { u.UserId, u.Username, u.Email, u.RegistrationDate });

            return Ok(returnList);
        }

        // HTTP GET: /api/users/{ id }
        // <summary>
        //  API method to get information about an existing user
        // </summary>
        [HttpGet("{id}")]
        public async Task< IActionResult > GetUser([FromRoute] int id)
        {
            // Find user with the provided Id in the database
            var user = await db.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found." });
            
            // Verify that the currently authenticated user is the same as the user whose details we're fetching
            var authUser = Int32.Parse(HttpContext.User.Identity.Name);
            if (user.UserId != authUser || id != authUser)
                return Unauthorized(new { message = "You are not authorized to view the details of that user. "});
            
            // Return user details back to client (purposefully excluding any password details)
            return Ok(new { user.UserId, user.Username, user.Email, user.RegistrationDate });
        }

        // HTTP PUT: /api/users/{ id }
        // <summary>
        //  API method to update an existing user
        // </summary>
        [HttpPut("{id}")]
        public async Task< IActionResult > UpdateUser([FromRoute] int id, [FromBody] UserDto userData)
        {
            // Find the user we want to update in the database
            var user = await db.Users.SingleOrDefaultAsync(x => x.UserId == id);
            if (user == null)
                return NotFound(new { message = "User not found." });
            
            // Verify that the currently authenticated user is the same as the user we're updating
            var authUser = Int32.Parse(HttpContext.User.Identity.Name);
            if (user.UserId != authUser || id != authUser)
                return Unauthorized(new { message = "You are not authorized to edit that user." });
            
            // Argument verification
            if (string.IsNullOrWhiteSpace(userData.Username) && string.IsNullOrWhiteSpace(userData.Email) && string.IsNullOrWhiteSpace(userData.Password))
                return BadRequest(new { message = "Nothing entered." });
            
            // Update username if a new username was provided
            if (!string.IsNullOrWhiteSpace(userData.Username) && (userData.Username != user.Username))
            {
                // Verify that the new username is not already in use
                var usernameTaken = await db.Users.AnyAsync(x => x.Username == userData.Username);
                if (usernameTaken)
                    return BadRequest(new { message = "Username \"" + userData.Username + "\" is already in use." });
                
                // If the provided username is available update the user's stored username
                user.Username = userData.Username;
            }

            // Update email if a new email address was provided
            if (!string.IsNullOrWhiteSpace(userData.Email) && (userData.Email != user.Email))
            {
                // Verify that the new email address is not already in use
                var emailTaken = await db.Users.AnyAsync(x => x.Email == userData.Email);
                if (emailTaken)
                    return BadRequest(new { message = "Email \"" + userData.Email + "\" is already in use." });
                
                // If the provided email is available update the user's stored email address
                user.Email = userData.Email;
            }

            // Update password if a new one was provided
            if (!string.IsNullOrWhiteSpace(userData.Password))
                user.CreatePasswordHash(userData.Password);

            db.Users.Update(user);
            await db.SaveChangesAsync();

            return Ok(new { id = user.UserId, user.Username, message = "User account successfully updated." });
        }

        // HTTP DELETE: /api/users/{ id }
        // <summary>
        //  API method to delete a user
        // </summary>
        [HttpDelete("{id}")]
        public async Task< IActionResult > DeleteUser([FromRoute] int id)
        {
            // Find the user to delete in the database
            var user = await db.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found." });
            
            // Verify that the currently authenticated user is the same as the user who we're deleting
            var authUser = Int32.Parse(HttpContext.User.Identity.Name);
            if (user.UserId != authUser || id != authUser)
                return Unauthorized(new { message = "You are not authorized to delete that user account." });
            
            // Remove the user from the database & save the changes
            db.Remove(user);
            await db.SaveChangesAsync();
            
            return Ok(new { message = "User account successfully deleted." });
        }
    }
}