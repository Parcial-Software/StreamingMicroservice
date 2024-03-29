﻿namespace StreamingMicroservice.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int UserId { get; set; }

        public User? User { get; set; }
        public List<PlaylistSong>? PlaylistSongs { get; set; }
    }
}
