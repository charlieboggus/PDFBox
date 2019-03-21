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
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using PDFBox.Api.Data;
using PDFBox.Api.Models;

namespace PDFBox.Api.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly PDFBoxContext db;

        public UserController(PDFBoxContext db)
        {
            this.db = db;
        }

        // HTTP GET: api/users/
        [HttpGet]
        public IEnumerable< User > Get()
        {
            return db.Users;
        }

        // HTTP GET: api/users/{id}
        [HttpGet("{id}", Name = "GetUser")]
        public async Task< IActionResult > GetUser([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var usr = await db.Users.SingleOrDefaultAsync(u => u.Id == id);
            if(usr == null)
                return NotFound();

            return Ok(usr);
        }

        // HTTP PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task< IActionResult > Put([FromRoute] int id, [FromBody] User usr)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if(id != usr.Id)
                return BadRequest();

            db.Entry(usr).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!UserExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/users/register
        [AllowAnonymous]
        [HttpPost, Route("register")]
        public async Task< IActionResult > Post([FromBody] User usr)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await db.Users.AddAsync(usr);
            await db.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = usr.Id }, usr);
        }

        // HTTP POST: api/users/auth
        // TODO: Authentication method

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task< IActionResult > Delete([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var usr = await db.Users.SingleOrDefaultAsync(u => u.Id == id);
            if(usr == null)
                return NotFound();
            
            db.Users.Remove(usr);
            await db.SaveChangesAsync();

            return Ok(usr);
        }

        private bool UserExists(int id)
        {
            return db.Users.Any(e => e.Id == id);
        }
    }
}