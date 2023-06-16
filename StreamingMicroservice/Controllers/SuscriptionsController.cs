using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StreamingMicroservice.Data;
using StreamingMicroservice.Models;

namespace StreamingMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuscriptionsController : ControllerBase
    {
        private readonly DataContext _context;

        public SuscriptionsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Suscriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Suscription>>> GetSuscriptions()
        {
          if (_context.Suscriptions == null)
          {
              return NotFound();
          }
            return await _context.Suscriptions.ToListAsync();
        }

        // GET: api/Suscriptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Suscription>> GetSuscription(int id)
        {
          if (_context.Suscriptions == null)
          {
              return NotFound();
          }
            var suscription = await _context.Suscriptions.FindAsync(id);

            if (suscription == null)
            {
                return NotFound();
            }

            return suscription;
        }

        // PUT: api/Suscriptions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSuscription(int id, Suscription suscription)
        {
            if (id != suscription.Id)
            {
                return BadRequest();
            }

            _context.Entry(suscription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SuscriptionExists(id))
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

        // POST: api/Suscriptions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Suscription>> PostSuscription(Suscription suscription)
        {
          if (_context.Suscriptions == null)
          {
              return Problem("Entity set 'DataContext.Suscriptions'  is null.");
          }
            _context.Suscriptions.Add(suscription);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSuscription", new { id = suscription.Id }, suscription);
        }

        // DELETE: api/Suscriptions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSuscription(int id)
        {
            if (_context.Suscriptions == null)
            {
                return NotFound();
            }
            var suscription = await _context.Suscriptions.FindAsync(id);
            if (suscription == null)
            {
                return NotFound();
            }

            _context.Suscriptions.Remove(suscription);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SuscriptionExists(int id)
        {
            return (_context.Suscriptions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
