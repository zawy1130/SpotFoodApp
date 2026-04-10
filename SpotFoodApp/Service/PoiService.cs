using SpotFoodApp.DTO;
using System.Net.Http.Json;

namespace SpotFoodApp.Service;

public class PoiService
{
    private readonly HttpClient _httpClient;

    public PoiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Lấy danh sách tất cả POI theo ngôn ngữ (dùng cho bản đồ)
    public async Task<List<PoiDto>> GetAllPoisAsync(string language = "vi")
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<PoiDto>>($"api/pois?language={language}")
                   ?? new List<PoiDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetAllPoisAsync Error: {ex.Message}");
            return new List<PoiDto>();
        }
    }

    // Lấy chi tiết một POI theo ngôn ngữ (dùng khi click marker để mở pop-up)
    public async Task<PoiDetailDto?> GetPoiDetailAsync(int id, string language = "vi")
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PoiDetailDto>($"api/pois/{id}?language={language}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetPoiDetailAsync Error (ID={id}, Language={language}): {ex.Message}");
            return null;
        }
    }
}