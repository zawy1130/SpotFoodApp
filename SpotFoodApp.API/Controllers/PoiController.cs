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

    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearby(double lat, double lng)
    {
        var pois = await _context.Pois
            .Where(p => Math.Abs(p.Latitude - lat) < 0.01 &&
                        Math.Abs(p.Longitude - lng) < 0.01)
            .Select(p => new {
                p.PoiId,
                p.Name,
                p.Latitude,
                p.Longitude
            })
            .ToListAsync();

        return Ok(pois);
    }
}