using Microsoft.Maui.Media;
using System.Threading;

namespace SpotFoodApp.Service
{
    public class TTSService
    {
        private CancellationTokenSource? _cts;

        /// <summary>
        /// Thuyết minh bằng TTS với hỗ trợ ngôn ngữ, giọng đọc và tốc độ
        /// </summary>
        public async Task Speak(string text, string languageCode, string? voiceName = null, double rate = 1.0)
        {
            if (string.IsNullOrEmpty(text)) return;

            CancelCurrent();

            _cts = new CancellationTokenSource();

            try
            {
                var locales = await TextToSpeech.Default.GetLocalesAsync();

                Locale? selectedLocale = null;

                if (!string.IsNullOrEmpty(voiceName))
                {
                    selectedLocale = locales.FirstOrDefault(l =>
                        l.Name.Equals(voiceName, StringComparison.OrdinalIgnoreCase));
                }

                if (selectedLocale == null)
                {
                    selectedLocale = locales.FirstOrDefault(l =>
                        l.Language.StartsWith(languageCode, StringComparison.OrdinalIgnoreCase));
                }

                var options = new SpeechOptions
                {
                    Locale = selectedLocale
                    // Volume và Pitch có thể thêm sau nếu cần
                };

                await TextToSpeech.Default.SpeakAsync(text, options, _cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Người dùng dừng TTS → bỏ qua
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TTS Error: {ex.Message}");
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }
        }

        /// <summary>
        /// Dừng TTS đang chạy
        /// </summary>
        public void CancelCurrent()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
        }
    }
}