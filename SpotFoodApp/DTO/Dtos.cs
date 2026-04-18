using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotFoodApp.DTO
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }

    public class PoiDto
    {
        public int PoiId { get; set; }
        public string? Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public string? Address { get; set; }
    }

    public class PoiDetailDto
    {
        public int PoiId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? ImageUrl { get; set; }
        public string? MapLink { get; set; }
        public string? AudioUrl { get; set; }
    }

    public class Place
    {
        public int poiId { get; set; }
        public string name { get; set; } = "";
        public double lat { get; set; }
        public double lng { get; set; }
        public int? categoryId { get; set; }
    }

    public class LanguageOption
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
    }
}
