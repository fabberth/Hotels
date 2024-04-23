
using Hotels.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hotels.AppDbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Hotel> Hotels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Configuration>().ToTable("AppConfigurations");
            modelBuilder.Entity<AppUser>().ToTable("AppUsers");

            modelBuilder.Entity<Hotel>().ToTable("Hotels").HasKey(p => p.IdHotel);

            modelBuilder.Entity<Room>().ToTable("Rooms")
                .HasOne(r => r.Hotel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(x=> x.IdHotel);

            

        }
    }
}
