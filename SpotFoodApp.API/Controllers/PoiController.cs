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

    // 📍 1. Lấy danh sách POI theo ngôn ngữ (dùng cho map)
    [HttpGet]
    public async Task<IActionResult> GetAll(string language = "vi")
    {
        var deviceId = Request.Headers["X-Device-Id"].FirstOrDefault() ?? "unknown";

        var pois = await _context.Pois
            .AsNoTracking()
            .Select(p => new
            {
                p.PoiId,

                // 🔥 đa ngôn ngữ
                Name = p.Translations!
                    .Where(t => t.LanguageCode == language)
                    .Select(t => t.Name)
                    .FirstOrDefault() ?? p.Name,

                p.Latitude,
                p.Longitude,
                p.CategoryId,
                p.ImageUrl,
                p.Address,

                // 🔥 QUAN TRỌNG: thêm Priority
                Priority = p.Priority
            })
            .ToListAsync();

        await _context.ApiAccessLogs.AddAsync(new ApiAccessLog
        {
            DeviceId = deviceId,
            Endpoint = "/api/pois",
            HttpMethod = "GET",
            PoiId = null,
            StatusCode = 200,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return Ok(pois);
    }

    // 📄 2. Chi tiết POI theo ngôn ngữ + audio
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(int id, string language = "vi")
    {
        var deviceId = Request.Headers["X-Device-Id"].FirstOrDefault() ?? "unknown";

        var poi = await _context.Pois
            .AsNoTracking()
            .Include(p => p.Translations!)
            .Include(p => p.Contents!)
                .ThenInclude(c => c.Audio)
            .FirstOrDefaultAsync(p => p.PoiId == id);

        if (poi == null)
        {
            await _context.ApiAccessLogs.AddAsync(new ApiAccessLog
            {
                DeviceId = deviceId,
                Endpoint = "/api/pois/" + id,
                HttpMethod = "GET",
                PoiId = id,
                StatusCode = 404,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return NotFound();
        }

        var translation = poi.Translations?
            .FirstOrDefault(t => t.LanguageCode == language)
            ?? poi.Translations?.FirstOrDefault(t => t.LanguageCode == "vi");

        var content = poi.Contents?.FirstOrDefault();

        var result = new
        {
            poi.PoiId,
            Name = translation?.Name ?? poi.Name,
            Description = translation?.Description ?? content?.Description ?? "",
            Address = translation?.Address ?? poi.Address,
            ImageUrl = poi.ImageUrl,
            MapLink = poi.MapLink,

            // 🔥 thêm Priority
            Priority = poi.Priority,

            AudioUrl = (language == "vi" && content?.Audio != null)
                       ? content.Audio.FilePath ?? ""
                       : ""
        };

        await _context.ApiAccessLogs.AddAsync(new ApiAccessLog
        {
            DeviceId = deviceId,
            Endpoint = "/api/pois/" + id,
            HttpMethod = "GET",
            PoiId = id,
            StatusCode = 200,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return Ok(result);
    }

    // 📂 4. Lọc theo category
    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var deviceId = Request.Headers["X-Device-Id"].FirstOrDefault() ?? "unknown";

        var pois = await _context.Pois
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId)
            .Select(p => new
            {
                p.PoiId,
                p.Name,
                p.Latitude,
                p.Longitude,
                p.ImageUrl,

                // 🔥 thêm Priority
                Priority = p.Priority
            })
            .ToListAsync();

        await _context.ApiAccessLogs.AddAsync(new ApiAccessLog
        {
            DeviceId = deviceId,
            Endpoint = "/api/pois/category/" + categoryId,
            HttpMethod = "GET",
            PoiId = null,
            StatusCode = 200,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return Ok(pois);
    }
}