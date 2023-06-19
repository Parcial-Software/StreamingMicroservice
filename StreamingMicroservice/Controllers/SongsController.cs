using System;
using System.Collections.Generic;
using System.Data;
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
    public class SongsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IBlobService _blob;
        private readonly IBusSender _sender;

        public SongsController(DataContext context, IBlobService blob, IBusSender sender = null)
        {
            _context = context;
            _blob = blob;
            _sender = sender;
        }

        // GET: api/Songs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Song>>> GetSongs()
        {
          if (_context.Songs == null)
          {
              return NotFound();
          }
            return await _context.Songs.ToListAsync();
        }

        [HttpGet("gender/{genderId}")]
        public async Task<ActionResult<List<Song>>> GetSongsByGenderId(int genderId)
        {
            if (_context.Genders == null)
            {
                return NotFound();
            }
            var gender = await _context.Genders.FindAsync(genderId);
            if (gender == null)
            {
                return NotFound("GenderId no found");
            }

            var songs = await _context.Songs
                .Where(x => x.GenderId == genderId)
                .ToListAsync();

            return songs;
        }

        // GET: api/Songs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Song>> GetSong(int id)
        {
          if (_context.Songs == null)
          {
              return NotFound();
          }
            var song = await _context.Songs.FindAsync(id);

            if (song == null)
            {
                return NotFound();
            }

            return song;
        }

        // PUT: api/Songs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSong(int id, Song song)
        {
            if (id != song.Id)
            {
                return BadRequest();
            }

            _context.Entry(song).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                await _sender.SendMessage(new Message<Song>
                {
                    Data = song,
                    Action = (int)MessageAction.Update,
                    Table = "Songs"
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SongExists(id))
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
        public async Task<ActionResult<Song>> UploadPhoto(int id, IFormFile document)
        {
            var song = await _context.Songs.FindAsync(id);

            if (song == null)
            {
                return NotFound("Song not found");
            }

            var imageUrl = await _blob.SaveToBlob("song", document.FileName, document.OpenReadStream());

            song.ImageUrl = imageUrl;
            _context.Songs.Update(song);

            await _context.SaveChangesAsync();

            await _sender.SendMessage(new Message<Song>
            {
                Data = song,
                Action = (int)MessageAction.Update,
                Table = "Songs"
            });

            return Ok(song);
        }

        [HttpPut("uploadFile/{id}")]
        public async Task<ActionResult<Song>> UploadFile(int id, IFormFile document)
        {
            var song = await _context.Songs.FindAsync(id);

            if (song == null)
            {
                return NotFound("Song not found");
            }

            var fileUrl = await _blob.SaveToBlob("song", document.FileName, document.OpenReadStream());

            song.FileUrl = fileUrl;
            _context.Songs.Update(song);

            await _context.SaveChangesAsync();

            await _sender.SendMessage(new Message<Song>
            {
                Data = song,
                Action = (int)MessageAction.Update,
                Table = "Songs"
            });

            return Ok(song);
        }

        // POST: api/Songs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Song>> PostSong(Song song)
        {
          if (_context.Songs == null)
          {
              return Problem("Entity set 'DataContext.Songs'  is null.");
          }
            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            await _sender.SendMessage(new Message<Song>
            {
                Data = song,
                Action = (int)MessageAction.Create,
                Table = "Songs"
            });

            return CreatedAtAction("GetSong", new { id = song.Id }, song);
        }

        // DELETE: api/Songs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            if (_context.Songs == null)
            {
                return NotFound();
            }
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();

            await _sender.SendMessage(new Message<Song>
            {
                Data = song,
                Action = (int)MessageAction.Delete,
                Table = "Songs"
            });

            return NoContent();
        }

        private bool SongExists(int id)
        {
            return (_context.Songs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
