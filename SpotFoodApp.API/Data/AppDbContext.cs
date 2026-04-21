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

        public DbSet<PoiTranslation> PoiTranslations { get; set; }
        public DbSet<ApiAccessLog> ApiAccessLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // POI - POI_TRANSLATION (1-n)
            modelBuilder.Entity<Poi>()
                .HasMany(p => p.Translations)
                .WithOne(t => t.Poi)
                .HasForeignKey(t => t.PoiId)
                .OnDelete(DeleteBehavior.Cascade);

            // POI - POI_CONTENT (1-n)
            modelBuilder.Entity<Poi>()
                .HasMany(p => p.Contents)
                .WithOne(pc => pc.Poi)
                .HasForeignKey(pc => pc.PoiId)
                .OnDelete(DeleteBehavior.Cascade);

            // POI_CONTENT - AUDIO_FILE (n-1)
            modelBuilder.Entity<PoiContent>()
                .HasOne(pc => pc.Audio)
                .WithMany(a => a.Contents)   // Nếu AudioFile có ICollection<PoiContent>
                .HasForeignKey(pc => pc.AudioId)
                .OnDelete(DeleteBehavior.SetNull);

            // UNIQUE constraint cho (poi_id + language_code)
            modelBuilder.Entity<PoiTranslation>()
                .HasIndex(t => new { t.PoiId, t.LanguageCode })
                .IsUnique();
        }
    }
}