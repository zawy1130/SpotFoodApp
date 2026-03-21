using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotFoodApp.API.Data;
using SpotFoodApp.API.DTO;

[Route("api/[controller]")]
[ApiController]
public class AudioGuideController : ControllerBase
{
    private readonly AppDbContext _context;

    public AudioGuideController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/AudioGuide/all
    [HttpGet("all")]
    public async Task<IActionResult> GetAllGuides()
    {
        try
        {
            var data = await _context.PoiContents
                .Include(pc => pc.Poi)     // lấy tên địa điểm
                .Include(pc => pc.Audio)   // lấy audio
                .Select(pc => new AudioGuideDto
                {
                    PoiId = pc.PoiId,
                    PoiName = pc.Poi.Name,
                    Description = pc.Description,
                    AudioUrl = pc.Audio != null ? pc.Audio.FilePath : null
                })
                .ToListAsync();

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Lỗi server: " + ex.Message);
        }
    }
}