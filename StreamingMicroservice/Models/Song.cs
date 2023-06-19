namespace StreamingMicroservice.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Artist { get; set; } = null!;
        public string Lyrics { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public decimal Price { get; set; } = 0;
        public int Reproductions { get; set; }
        public int Downloads { get; set; }
        public int AlbumId { get; set; }
        public int GenderId { get; set; }
        
        public Album? Album { get; set; } 
        public Gender? Gender { get; set; } 
    }
}
