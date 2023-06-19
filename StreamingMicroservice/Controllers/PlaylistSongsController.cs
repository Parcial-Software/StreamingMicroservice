using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StreamingMicroservice.Data;
using StreamingMicroservice.Models;
using StreamingMicroservice.Services.Bus;

namespace StreamingMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistSongsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IBusSender _sender;

        public PlaylistSongsController(DataContext context, IBusSender sender = null)
        {
            _context = context;
            _sender = sender;
        }

        // GET: api/PlaylistSongs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlaylistSong>>> GetPlaylistSong()
        {
          if (_context.PlaylistSong == null)
          {
              return NotFound();
          }
            return await _context.PlaylistSong.ToListAsync();
        }

        // GET: api/PlaylistSongs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlaylistSong>> GetPlaylistSong(int id)
        {
          if (_context.PlaylistSong == null)
          {
              return NotFound();
          }
            var playlistSong = await _context.PlaylistSong.FindAsync(id);

            if (playlistSong == null)
            {
                return NotFound();
            }

            return playlistSong;
        }

        // PUT: api/PlaylistSongs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlaylistSong(int id, PlaylistSong playlistSong)
        {
            if (id != playlistSong.Id)
            {
                return BadRequest();
            }

            _context.Entry(playlistSong).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                await _sender.SendMessage(new Message<PlaylistSong>
                {
                    Data = playlistSong,
                    Action = (int)MessageAction.Update,
                    Table = "PlaylistSongs"
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaylistSongExists(id))
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

        // POST: api/PlaylistSongs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PlaylistSong>> PostPlaylistSong(PlaylistSong playlistSong)
        {
          if (_context.PlaylistSong == null)
          {
              return Problem("Entity set 'DataContext.PlaylistSong'  is null.");
          }
            _context.PlaylistSong.Add(playlistSong);
            await _context.SaveChangesAsync();

            await _sender.SendMessage(new Message<PlaylistSong>
            {
                Data = playlistSong,
                Action = (int)MessageAction.Create,
                Table = "PlaylistSongs"
            });

            return CreatedAtAction("GetPlaylistSong", new { id = playlistSong.Id }, playlistSong);
        }

        // DELETE: api/PlaylistSongs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylistSong(int id)
        {
            if (_context.PlaylistSong == null)
            {
                return NotFound();
            }
            var playlistSong = await _context.PlaylistSong.FindAsync(id);
            if (playlistSong == null)
            {
                return NotFound();
            }

            _context.PlaylistSong.Remove(playlistSong);
            await _context.SaveChangesAsync();

            await _sender.SendMessage(new Message<PlaylistSong>
            {
                Data = playlistSong,
                Action = (int)MessageAction.Delete,
                Table = "PlaylistSongs"
            });

            return NoContent();
        }

        private bool PlaylistSongExists(int id)
        {
            return (_context.PlaylistSong?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
