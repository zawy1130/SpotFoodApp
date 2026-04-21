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
        var categories = await _context.Category
            .Select(c => new
            {
                c.CategoryId,
                c.CategoryName,
            })
            .ToListAsync();

        await _context.ApiAccessLogs.AddAsync(new ApiAccessLog
        {
            Endpoint = "/api/categories",
            Method = "GET",
            AccessedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return Ok(categories);
    }
}
