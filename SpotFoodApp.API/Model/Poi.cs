using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpotFoodApp.API.Model
{
    [Table("POI")]
    public class Poi
    {
        [Key]
        [Column("poi_id")]
        public int PoiId { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("latitude")]
        public double Latitude { get; set; }

        [Column("longitude")]
        public double Longitude { get; set; }
        
        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [Column("map_link")]
        public string? MapLink { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [Column("category_id")]
        public int? CategoryId { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        public Category? Category { get; set; }
        // navigation
        public ICollection<PoiContent>? Contents { get; set; }
    }
}