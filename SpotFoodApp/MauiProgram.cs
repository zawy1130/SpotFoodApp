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

            // 🔥 HttpClient (API) - remote hosting (somee)
            builder.Services.AddScoped(sp =>
                new HttpClient
                {
                    // Per request: use the provided base address for the hosted API
                    BaseAddress = new Uri("https://sony-site.somee.com/")
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