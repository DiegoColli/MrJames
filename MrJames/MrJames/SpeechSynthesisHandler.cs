using Microsoft.CognitiveServices.Speech;

namespace MrJames;

class SpeechSynthesisHandler(SpeechSynthesizer synthesizer)
{
    public async Task SpeakAsync(string text)
    {
        using var result = await synthesizer.SpeakTextAsync(text);
        if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
            Console.WriteLine($"ErrorCode: {cancellation.ErrorCode}\nErrorDetails: {cancellation.ErrorDetails}");
        }
    }
}