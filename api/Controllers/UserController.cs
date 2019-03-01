using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PDFBox.Api.Data;
using PDFBox.Api.Models;

namespace PDFBox.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext db;

        public UserController(UserContext db)
        {
            this.db = db;
        }

        // GET: api/users/
        [HttpGet]
        public IEnumerable< User > Get()
        {
            return db.Users;
        }

        // GET: api/users/{id}
        [HttpGet("{id}", Name = "GetUser")]
        public async Task< IActionResult > GetUser([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var usr = await db.Users.SingleOrDefaultAsync(u => u.UserId == id);
            if(usr == null)
                return NotFound();

            return Ok(usr);
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task< IActionResult > Put([FromRoute] int id, [FromBody] User usr)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if(id != usr.UserId)
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

        // POST: api/users/
        [HttpPost]
        public async Task< IActionResult > Post([FromBody] User usr)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await db.Users.AddAsync(usr);
            await db.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = usr.UserId }, usr);
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task< IActionResult > Delete([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var usr = await db.Users.SingleOrDefaultAsync(u => u.UserId == id);
            if(usr == null)
                return NotFound();
            
            db.Users.Remove(usr);
            await db.SaveChangesAsync();

            return Ok(usr);
        }

        private bool UserExists(int id)
        {
            return db.Users.Any(e => e.UserId == id);
        }
    }
}