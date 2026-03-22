using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotFoodApp.API.Data;

[ApiController]
[Route("api/poi")]
public class PoiController : ControllerBase
{
    private readonly AppDbContext _context;

    public PoiController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _context.Pois
            .Include(p => p.Contents)
            .ThenInclude(c => c.Audio)
            .ToListAsync();

        return Ok(data);
    }

    // API returns nearby POIs, based on passed-in latitude and longtitude.
    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearby(double lat, double lon, double radiusInKm = 2.0)
    {
        // Lấy toàn bộ danh sách POI (Hoặc lọc sơ bộ theo vùng để tối ưu hiệu năng)
        var allPois = await _context.Pois
            .Include(p => p.Contents)
            .ThenInclude(c => c.Audio)
            .ToListAsync();

        // Lọc các quán nằm trong bán kính cho phép
        var nearbyPois = allPois.Where(p =>
        {
            var distance = CalculateDistance(lat, lon, p.Latitude, p.Longitude);
            return distance <= radiusInKm;
        }).ToList();

        if (nearbyPois.Any())
        {
            return Ok(nearbyPois);
        }
        else
        {
            //return Ok(new
            //{
            //    Message = "No shops found nearby",
            //    Latitude = lat,
            //    Longitude = lon,
            //});
            return NoContent();
        }
        
    }

    // Hàm tính khoảng cách giữa 2 tọa độ (Đơn vị: Km)
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var d1 = lat1 * (Math.PI / 180.0);
        var num1 = lon1 * (Math.PI / 180.0);
        var d2 = lat2 * (Math.PI / 180.0);
        var num2 = lon2 * (Math.PI / 180.0) - num1;
        var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                 Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

        return 6371.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
    }
}