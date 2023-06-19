using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StreamingMicroservice.Data;
using StreamingMicroservice.Models;
using StreamingMicroservice.Services.Blob;
using StreamingMicroservice.Services.Bus;

namespace StreamingMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GendersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IBlobService _blob;
        private readonly IBusSender _sender;

        public GendersController(DataContext context, IBlobService blob, IBusSender sender = null)
        {
            _context = context;
            _blob = blob;
            _sender = sender;
        }

        // GET: api/Genders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gender>>> GetGenders()
        {
            if (_context.Genders == null)
            {
                return NotFound();
            }
            return await _context.Genders.ToListAsync();
        }

        // GET: api/Genders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Gender>> GetGender(int id)
        {
            if (_context.Genders == null)
            {
                return NotFound();
            }
            var gender = await _context.Genders.FindAsync(id);

            if (gender == null)
            {
                return NotFound();
            }

            return gender;
        }

        [HttpPut("uploadImage/{id}")]
        public async Task<ActionResult<Gender>> UploadPhoto(int id, IFormFile document)
        {
            var gender = await _context.Genders.FindAsync(id);

            if (gender == null)
            {
                return NotFound("Gender not found");
            }

            var imageUrl = await _blob.SaveToBlob("gender", document.FileName, document.OpenReadStream());
            
            gender.ImageUrl = imageUrl;
            _context.Genders.Update(gender);

            await _context.SaveChangesAsync();

            await _sender.SendMessage(new Message<Gender>
            {
                Data = gender,
                Action = (int)MessageAction.Update,
                Table = "Genders"
            });

            return Ok(gender);                                      
        }

        // PUT: api/Genders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGender(int id, Gender gender)
        {
            if (id != gender.Id)
            {
                return BadRequest();
            }

            _context.Entry(gender).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                await _sender.SendMessage(new Message<Gender>
                {
                    Data = gender,
                    Action = (int)MessageAction.Update,
                    Table = "Genders"
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenderExists(id))
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

        // POST: api/Genders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Gender>> PostGender(Gender gender)
        {
            if (_context.Genders == null)
            {
                return Problem("Entity set 'DataContext.Genders'  is null.");
            }

            gender.ImageUrl = "";

            _context.Genders.Add(gender);
            await _context.SaveChangesAsync();

            await _sender.SendMessage(new Message<Gender>
            {
                Data = gender,
                Action = (int)MessageAction.Create,
                Table = "Genders"
            });

            return CreatedAtAction("GetGender", new { id = gender.Id }, gender);
        }

        // DELETE: api/Genders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGender(int id)
        {
            if (_context.Genders == null)
            {
                return NotFound();
            }
            var gender = await _context.Genders.FindAsync(id);
            if (gender == null)
            {
                return NotFound();
            }

            _context.Genders.Remove(gender);
            await _context.SaveChangesAsync();

            await _sender.SendMessage(new Message<Gender>
            {
                Data = gender,
                Action = (int)MessageAction.Delete,
                Table = "Genders"
            });

            return NoContent();
        }

        private bool GenderExists(int id)
        {
            return (_context.Genders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
