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

            // Api Access Log
            modelBuilder.Entity<ApiAccessLog>(entity =>
            {
                entity.ToTable("API_ACCESS_LOG");
                entity.HasKey(x => x.LogId);

                entity.Property(x => x.LogId).HasColumnName("log_id");
                entity.Property(x => x.DeviceId).HasColumnName("device_id");
                entity.Property(x => x.Endpoint).HasColumnName("endpoint");
                entity.Property(x => x.HttpMethod).HasColumnName("http_method");
                entity.Property(x => x.PoiId).HasColumnName("poi_id");
                entity.Property(x => x.StatusCode).HasColumnName("status_code");
                entity.Property(x => x.CreatedAt).HasColumnName("created_at");
            });
        }
    }
}