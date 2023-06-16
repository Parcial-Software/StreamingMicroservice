namespace StreamingMicroservice.Models
{
    public class History
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SongId { get; set; }

        public User? User { get; set; }
        public Song? Song { get; set; } 
    }
}
