using Microsoft.EntityFrameworkCore;
using StreamingMicroservice.Models;

namespace StreamingMicroservice.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {

            
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Suscription> Suscriptions { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Favorite> Favorite { get; set; } = default!;
        public DbSet<History> History { get; set; } = default!;



    }
}
