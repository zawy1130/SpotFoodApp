using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotFoodApp.API.Data;
using SpotFoodApp.API.DTO;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/categories
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var deviceId = Request.Headers["X-Device-Id"].FirstOrDefault() ?? "unknown";

        var categories = await _context.Category
            .Select(c => new
            {
                c.CategoryId,
                c.CategoryName,
            })
            .ToListAsync();

        await _context.ApiAccessLogs.AddAsync(new ApiAccessLog
        {
            DeviceId = deviceId,
            Endpoint = "/api/categories",
            HttpMethod = "GET",
            PoiId = null,
            StatusCode = 200,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return Ok(categories);
    }
}
