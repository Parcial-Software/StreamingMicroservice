using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StreamingMicroservice.Data;
using StreamingMicroservice.DTOs;
using StreamingMicroservice.Models;
using StreamingMicroservice.Services.Bus;

namespace StreamingMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IBusSender _sender;

        public UsersController(DataContext context, IBusSender sender = null)
        {
            _context = context;
            _sender = sender;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                await _sender.SendMessage(new Message<User>
                {
                    Data = user,
                    Action = (int)MessageAction.Update,
                    Table = "Users"
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'DataContext.Users'  is null.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _sender.SendMessage(new Message<User>
            {
                Data = user,
                Action = (int)MessageAction.Create,
                Table = "Users"
            });

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            await _sender.SendMessage(new Message<User>
            {
                Data = user,
                Action = (int)MessageAction.Delete,
                Table = "Users"
            });

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost("Login")]
        public async Task<ActionResult<User>> Login(LoginRequest loginRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(
                x => x.Email == loginRequest.Email);

            if (user == null)
            {
                return NotFound("Email not found");
            }

            if (user.Password != loginRequest.Password)
            {
                return BadRequest("Invalid Password");
            }

            return Ok(user);
        }
    }
}
