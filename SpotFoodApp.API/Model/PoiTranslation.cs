using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpotFoodApp.API.Model
{
    [Table("POI_TRANSLATION")]
    public class PoiTranslation
    {
        [Key]
        [Column("translation_id")]
        public int TranslationId { get; set; }

        [Column("poi_id")]
        public int PoiId { get; set; }

        [Column("language_code")]
        public string LanguageCode { get; set; } = "vi";   // vi, en, ja, fr, ko...

        [Column("name")]
        public string? Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        // Navigation
        public Poi? Poi { get; set; }
    }
}