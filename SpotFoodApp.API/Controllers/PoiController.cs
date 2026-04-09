using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotFoodApp.API.Data;

[ApiController]
[Route("api/pois")]
public class PoisController : ControllerBase
{
    private readonly AppDbContext _context;

    public PoisController(AppDbContext context)
    {
        _context = context;
    }

    // 📍 1. Lấy danh sách POI (dùng cho map)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var pois = await _context.Pois
            .AsNoTracking()
            .Select(p => new
            {
                p.PoiId,
                p.Name,
                p.Latitude,
                p.Longitude,
                p.CategoryId
            })
            .ToListAsync();

        return Ok(pois);
    }

    // 📄 2. Chi tiết POI + audio
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var poi = await _context.Pois
            .AsNoTracking()
            .Include(p => p.Contents!)
            .ThenInclude(c => c.Audio)
            .FirstOrDefaultAsync(p => p.PoiId == id);

        if (poi == null)
            return NotFound();

        var content = poi.Contents?.FirstOrDefault();

        var result = new
        {
            poi.PoiId,
            poi.Name,
            poi.Latitude,
            poi.Longitude,
            poi.ImageUrl,
            poi.Address,
            poi.MapLink,

            Description = content?.Description ?? "",
            AudioUrl = content?.Audio?.FilePath ?? ""
        };

        return Ok(result);
    }


    //// 🔥 3. POI gần vị trí (GPS)
    //// GET: api/pois/nearby?lat=...&lng=...&radius=100
    //[HttpGet("nearby")]
    //public async Task<IActionResult> GetNearby(double lat, double lng, double radius = 100)
    //{
    //    var pois = await _context.Pois
    //        .AsNoTracking()
    //        .Select(p => new
    //        {
    //            p.PoiId,
    //            p.Name,
    //            p.Latitude,
    //            p.Longitude
    //        })
    //        .ToListAsync();

    //    var result = pois
    //        .Select(p => new
    //        {
    //            p.PoiId,
    //            p.Name,
    //            p.Latitude,
    //            p.Longitude,
    //            Distance = GetDistance(lat, lng, p.Latitude, p.Longitude)
    //        })
    //        .Where(p => p.Distance <= radius)
    //        .OrderBy(p => p.Distance)
    //        .ToList();

    //    return Ok(result);
    //}

    // 📂 4. Lọc theo category
    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var pois = await _context.Pois
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId)
            .Select(p => new
            {
                p.PoiId,
                p.Name,
                p.Latitude,
                p.Longitude,
                p.ImageUrl
            })
            .ToListAsync();

        return Ok(pois);
    }

    // 📐 Hàm tính khoảng cách (mét)
    private double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6371000; // bán kính trái đất (m)

        double dLat = (lat2 - lat1) * Math.PI / 180;
        double dLon = (lon2 - lon1) * Math.PI / 180;

        double a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(lat1 * Math.PI / 180) *
            Math.Cos(lat2 * Math.PI / 180) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }
}