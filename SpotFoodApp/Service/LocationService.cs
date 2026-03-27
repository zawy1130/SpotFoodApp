using Microsoft.Maui.Devices.Sensors;

namespace SpotFoodApp.Service;

public class LocationService
{
    private bool _isListening = false;

    public async Task StartListening(Func<Location, Task> onLocationChanged)
    {
        if (_isListening) return;

        _isListening = true;

        while (_isListening)
        {
            try
            {
                var request = new GeolocationRequest(
                    GeolocationAccuracy.Best, //QUAN TRỌNG
                    TimeSpan.FromSeconds(5)
                );

                var location = await Geolocation.Default.GetLocationAsync(request);

                if (location != null)
                {
                    await onLocationChanged(location);
                }
                else
                {
                    Console.WriteLine("❌ Location NULL");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ GPS Error: {ex.Message}");
            }

            await Task.Delay(1000); // 1s mượt + ổn định
        }
    }

    public void StopListening()
    {
        _isListening = false;
    }
}