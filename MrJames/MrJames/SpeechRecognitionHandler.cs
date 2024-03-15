using Microsoft.CognitiveServices.Speech;

namespace MrJames;

class SpeechRecognitionHandler
{
    private readonly SpeechRecognizer _recognizer;

    public SpeechRecognitionHandler(SpeechRecognizer recognizer, Func<string, Task> processCommandAsync)
    {
        _recognizer = recognizer;
        _recognizer.Recognized += async (s, e) =>
        {
            await StopAsync();
            await processCommandAsync(e.Result.Text);
            await StartAsync();
        };
    }

    public async Task StartAsync() => await _recognizer.StartContinuousRecognitionAsync();
    public async Task StopAsync() => await _recognizer.StopContinuousRecognitionAsync();
}