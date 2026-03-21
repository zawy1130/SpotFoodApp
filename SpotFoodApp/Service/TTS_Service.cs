using Microsoft.Maui.Media;

namespace SpotFoodApp.Service
{
    public class TTSService
    {
        public async Task Speak(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            await TextToSpeech.SpeakAsync(text);
        }
    }
}