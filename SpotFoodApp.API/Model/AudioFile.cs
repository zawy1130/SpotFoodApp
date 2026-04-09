using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpotFoodApp.API.Model
{
    [Table("AUDIO_FILE")]
    public class AudioFile
    {
        [Key]
        [Column("audio_id")]
        public int AudioId { get; set; }

        [Column("file_path")]
        public string? FilePath { get; set; }

        [Column("duration")]
        public int? Duration { get; set; }

        [Column("file_size")]
        public int? FileSize { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        // navigation
        public ICollection<PoiContent>? Contents { get; set; }
    }
}