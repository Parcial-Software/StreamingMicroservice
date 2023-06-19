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
    public class AlbumsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IBlobService _blob;
        private readonly IBusSender _sender;

        public AlbumsController(DataContext context, IBlobService blob, IBusSender sender)
        {
            _context = context;
            _blob = blob;
            _sender = sender;
        }

        // GET: api/Albums
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Album>>> GetAlbums()
        {
          if (_context.Albums == null)
          {
              return NotFound();
          }
            return await _context.Albums.ToListAsync();
        }

        // GET: api/Albums/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Album>> GetAlbum(int id)
        {
          if (_context.Albums == null)
          {
              return NotFound();
          }
            var album = await _context.Albums.FindAsync(id);

            if (album == null)
            {
                return NotFound();
            }

            return album;
        }

        // PUT: api/Albums/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlbum(int id, Album album)
        {
            if (id != album.Id)
            {
                return BadRequest();
            }

            _context.Entry(album).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlbumExists(id))
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

        [HttpPut("uploadImage/{id}")]
        public async Task<ActionResult<Album>> UploadPhoto(int id, IFormFile document)
        {
            var album = await _context.Albums.FindAsync(id);

            if (album == null)
            {
                return NotFound("Album not found");
            }

            var imageUrl = await _blob.SaveToBlob("album", document.FileName, document.OpenReadStream());

            album.ImageUrl = imageUrl;
            _context.Albums.Update(album);

            await _context.SaveChangesAsync();

            return Ok(album);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Album>>> GetAlbumsByUserId(int userId)
        {
            if (_context.Albums == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("UserId no found");
            }

            var albums = await _context.Albums
                .Where(x => x.UserId == userId)
                .ToListAsync();

            return albums;
        }

        // POST: api/Albums
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Album>> PostAlbum(Album album)
        {
          if (_context.Albums == null)
          {
              return Problem("Entity set 'DataContext.Albums'  is null.");
          }
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            await _sender.SendMessage(new Message<Album>
            {
                Data = album,
                Action = (int) MessageAction.Create,
                Table = "Albums"
            });

            return CreatedAtAction("GetAlbum", new { id = album.Id }, album);
        }

        // DELETE: api/Albums/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlbum(int id)
        {
            if (_context.Albums == null)
            {
                return NotFound();
            }
            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }

            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AlbumExists(int id)
        {
            return (_context.Albums?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
