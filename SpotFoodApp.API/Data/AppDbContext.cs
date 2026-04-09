using Microsoft.EntityFrameworkCore;
using SpotFoodApp.API.Model;

namespace SpotFoodApp.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Poi> Pois { get; set; }
        public DbSet<PoiContent> PoiContents { get; set; }
        public DbSet<AudioFile> AudioFiles { get; set; }

        public DbSet<Category> Category{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // POI
            modelBuilder.Entity<Poi>()
                .HasKey(p => p.PoiId);

            // POI_CONTENT - POI (1-n)
            modelBuilder.Entity<PoiContent>()
                .HasOne(pc => pc.Poi)
                .WithMany(p => p.Contents)
                .HasForeignKey(pc => pc.PoiId);

            // POI_CONTENT - AUDIO_FILE (n-1)
            modelBuilder.Entity<PoiContent>()
                .HasOne(pc => pc.Audio)
                .WithMany(a => a.Contents)
                .HasForeignKey(pc => pc.AudioId);

            // UNIQUE (poi_id + language_code)
            modelBuilder.Entity<PoiContent>()
                .HasIndex(pc => new { pc.PoiId})
                .IsUnique();
        }
    }
}