using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SpotFoodApp.API.Model
{
    [Table("POI_CONTENT")]
    public class PoiContent
    {
        [Key]
        [Column("content_id")]
        public int ContentId { get; set; }

        [Column("poi_id")]
        public int PoiId { get; set; }

        [Column("title")]
        public string? Title { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("audio_id")]
        public int? AudioId { get; set; }

        // navigation
        [JsonIgnore]
        public Poi? Poi { get; set; }
        public AudioFile? Audio { get; set; }
    }
}