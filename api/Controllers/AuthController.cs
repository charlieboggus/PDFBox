using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using PDFBox.Api.Data;
using PDFBox.Api.Models;

namespace PDFBox.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserContext userDb;

        public AuthController(UserContext userDb)
        {
            this.userDb = userDb;
        }

        [HttpPost, Route("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            if(login == null)
                return BadRequest("Invalid client request");
            
            var user = userDb.Users.SingleOrDefault(u => (u.Username.Equals(login.Username) && u.Password.Equals(login.Password)));
            if(user != null)
            {
                var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("XRhPGhn7YeFj2j3THmxRKdCTTZVhUq"));
                var credentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
                var tokenOpts = new JwtSecurityToken(
                    issuer: "http://localhost:5000",    // TODO: this will need to be changed to an actual server when project is pushed to production
                    audience: "http://localhost:5000",  // TODO: this will need to be changed to an actual server when project is pushed to production
                    claims: new List< Claim >(),
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOpts);

                return Ok(new { Token = tokenString });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}