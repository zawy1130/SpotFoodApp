using SpotFoodApp.DTO;
using System.Net.Http.Json;
using Microsoft.Maui.Storage;
using System.Net.Http.Headers;

namespace SpotFoodApp.Service;

public class PoiService
{
    private readonly HttpClient _httpClient;

    public PoiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // new with headers.
    public async Task<List<PoiDto>> GetAllPoisAsync(string language = "vi")
    {
        try
        {
            var deviceId = GetOrCreateDeviceId();
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/pois?language={language}");
            request.Headers.Add("X-Device-Id", deviceId);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return new List<PoiDto>();

            return await response.Content.ReadFromJsonAsync<List<PoiDto>>()
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
            var deviceId = GetOrCreateDeviceId();
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/pois/{id}?language={language}");
            request.Headers.Add("X-Device-Id", deviceId);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<PoiDetailDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetPoiDetailAsync Error (ID={id}, Language={language}): {ex.Message}");
            return null;
        }
    }

    private string GetOrCreateDeviceId()
    {
        var savedDeviceId = Preferences.Get("device_id", string.Empty);

        if (!string.IsNullOrEmpty(savedDeviceId))
            return savedDeviceId;

        var newDeviceId = Guid.NewGuid().ToString();
        Preferences.Set("device_id", newDeviceId);

        return newDeviceId;
    }
}