using System.Text.RegularExpressions;
using Microsoft.CognitiveServices.Speech;
using OpenAI.ChatGpt;
using OpenAI.ChatGpt.Models.ChatCompletion.Messaging;

namespace MrJames;

class JamesService(OpenAiClient client, SpeechSynthesizer synthesizer, SpeechRecognizer recognizer) : IAsyncDisposable
{
    private bool _bypassCheck = false;
    
    public async Task InitializeAsync()
    {
        recognizer.Recognized += async (s, e) => await RecognizedAsync(e.Result.Text);
        await recognizer.StartContinuousRecognitionAsync();
    }

    private async Task RecognizedAsync(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            Console.WriteLine($"RECOGNIZED: {text}");
        
            var normalizedText = text.Replace("í", "i").Replace(",", " ").Replace("  ", " ").ToLower();

            if (normalizedText.StartsWith("e ai james desligar"))
            {
                await synthesizer.SpeakTextAsync("Até mais!");
                throw new Exception("Desligando...");
            }

            var regex = new Regex(@"\be\s+ai\s+james\b", RegexOptions.IgnoreCase);
            if (!regex.IsMatch(normalizedText) && _bypassCheck == false)
            {
                return;
            }

            var textWithoutGreeting = regex.Replace(text, string.Empty, 1).Trim(); // O '1' limita a substituição à primeira ocorrência
            
            Console.WriteLine($"FILTERED: {textWithoutGreeting}");

      
            string response = await client.GetChatCompletions(new UserMessage(textWithoutGreeting), maxTokens: 80, model: "gpt-3.5-turbo");
            Console.WriteLine($"RESPONSE: {response}");
            
            await recognizer.StopContinuousRecognitionAsync();
            await RepeatTextAsync(response);
            await recognizer.StartContinuousRecognitionAsync();
            StartBypassTimer();
        }
    }
    
    private async Task RepeatTextAsync(string text)
    {
        using var result = await synthesizer.SpeakTextAsync(text);
        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
        {
            Console.WriteLine("Áudio gerado com sucesso.");
        }
        else if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
            Console.WriteLine($"ErrorCode: {cancellation.ErrorCode}\nErrorDetails: {cancellation.ErrorDetails}");
        }
    }
    
    private void StartBypassTimer()
    {
        Console.WriteLine("Starting bypass");
        _bypassCheck = true;
        Task.Run(async () =>
        {
            await Task.Delay(10000);
            _bypassCheck = false;
            Console.WriteLine("Bypass finalizado");
        });
    }


    public async ValueTask DisposeAsync()
    {
        await recognizer.StopContinuousRecognitionAsync();
        await CastAndDispose(synthesizer);
        await CastAndDispose(recognizer);

        return;

        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
                await resourceAsyncDisposable.DisposeAsync();
            else
                resource.Dispose();
        }
    }
}