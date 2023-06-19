using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    public class PlaylistsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IBusSender _sender;

        public PlaylistsController(DataContext context, IBusSender sender = null)
        {
            _context = context;
            _sender = sender;
        }

        // GET: api/Playlists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylists()
        {
            if (_context.Playlists == null)
            {
                return NotFound();
            }
            return await _context.Playlists.ToListAsync();
        }

        // GET: api/Playlists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Playlist>> GetPlaylist(int id)
        {
            if (_context.Playlists == null)
            {
                return NotFound();
            }
            var playlist = await _context.Playlists.FindAsync(id);

            if (playlist == null)
            {
                return NotFound();
            }

            return playlist;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Playlist>>> GetPlaylistByUserId(int userId)
        {
            if (_context.Playlists == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("UserId no found");
            }

            var playlists = await _context.Playlists
                .Where(x => x.UserId == userId)
                .Include(x => x.PlaylistSongs)!
                    .ThenInclude(x => x.Song)
                .ToListAsync();

            return playlists;
        }

        // PUT: api/Playlists/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlaylist(int id, Playlist playlist)
        {
            if (id != playlist.Id)
            {
                return BadRequest();
            }

            _context.Entry(playlist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                await _sender.SendMessage(new Message<Playlist>
                {
                    Data = playlist,
                    Action = (int)MessageAction.Update,
                    Table = "Playlists"
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaylistExists(id))
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

        // POST: api/Playlists
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Playlist>> PostPlaylist(Playlist playlist)
        {
            if (_context.Playlists == null)
            {
                return Problem("Entity set 'DataContext.Playlists'  is null.");
            }

            var user = await _context.Users.FindAsync(playlist.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            await _sender.SendMessage(new Message<Playlist>
            {
                Data = playlist,
                Action = (int)MessageAction.Create,
                Table = "Playlists"
            });

            return CreatedAtAction("GetPlaylist", new { id = playlist.Id }, playlist);
        }

        // DELETE: api/Playlists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            if (_context.Playlists == null)
            {
                return NotFound();
            }
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }

            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();

            await _sender.SendMessage(new Message<Playlist>
            {
                Data = playlist,
                Action = (int)MessageAction.Delete,
                Table = "Playlists"
            });

            return NoContent();
        }

        private bool PlaylistExists(int id)
        {
            return (_context.Playlists?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
