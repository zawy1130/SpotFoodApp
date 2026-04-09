using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpotFoodApp.API.Model
{
    [Table("POI_CATEGORY")]
    public class Category
    {
        [Key]
        [Column("category_id")]
        public int CategoryId { get; set; }
        [Column("category_name")]
        public string? CategoryName { get; set; }

        // Navigation
        public ICollection<Poi>? Pois { get; set; }
    }
}
