using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using SpotFoodApp.Service;

namespace SpotFoodApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // 🔥 HttpClient (API)
            builder.Services.AddScoped(sp =>
                new HttpClient
                {
                    BaseAddress = new Uri("http://192.168.2.43:5205/")
                });

            // 🔥 Blazor
            builder.Services.AddMauiBlazorWebView();

            // 🔥 Services
            builder.Services.AddScoped<PoiService>();

            builder.Services.AddSingleton<IAudioManager, AudioManager>();
            builder.Services.AddSingleton<TTSService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}