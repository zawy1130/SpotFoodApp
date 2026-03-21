using System.Net.Http.Json;
using SpotFoodApp.DTO;


namespace SpotFoodApp.Service
{
    public class AudioService
    {
        private readonly HttpClient _http;

        public AudioService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<AudioGuideDto>> GetAllGuides()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<List<AudioGuideDto>>(
                    "api/AudioGuide/all"
                );

                return result ?? new List<AudioGuideDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("API ERROR: " + ex.Message);
                return new List<AudioGuideDto>(); // tránh null
            }
        }
    }
}