using SpotFoodApp.DTO;
using System.Net.Http.Json;
using static SpotFoodApp.Components.Pages.Mapview;

namespace SpotFoodApp.Service;

public class PoiService
{
    private readonly HttpClient _httpClient;

    public PoiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Lấy danh sách tất cả POI (dùng cho bản đồ)
    public async Task<List<PoiDto>> GetAllPoisAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<PoiDto>>("api/pois")
                   ?? new List<PoiDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetAllPoisAsync Error: {ex.Message}");
            return new List<PoiDto>();
        }
    }

    // Lấy chi tiết một POI (dùng khi click marker để mở pop-up)
    public async Task<PoiDetailDto?> GetPoiDetailAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PoiDetailDto>($"api/pois/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetPoiDetailAsync Error (ID={id}): {ex.Message}");
            return null;
        }
    }
}