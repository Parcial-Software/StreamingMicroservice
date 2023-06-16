namespace StreamingMicroservice.Models
{
    public class Album
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public int UserId { get; set; }

        public User? User { get; set; }
        public List<Song>? Songs { get; set; }
    }
}
