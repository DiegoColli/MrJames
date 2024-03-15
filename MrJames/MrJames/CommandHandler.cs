using System.Text.RegularExpressions;
using OpenAI.ChatGpt;
using OpenAI.ChatGpt.Models.ChatCompletion.Messaging;

namespace MrJames;

class CommandHandler(OpenAiClient client, SpeechSynthesisHandler synthesisHandler)
{
    private bool _bypassCheck;

    public async Task ProcessCommandAsync(string text)
    {
        var normalizedText = NormalizeText(text);
        if (ShouldProcessCommand(normalizedText))
        {
            var response = await client.GetChatCompletions(new UserMessage(normalizedText), maxTokens: 80, model: "gpt-3.5-turbo");
            Console.WriteLine($"RESPONSE: {response}");
            await synthesisHandler.SpeakAsync(response);
            StartBypassTimer();
        }
    }

    private string NormalizeText(string text) => text.Replace("í", "i").Replace(",", " ").Replace("  ", " ").ToLower();

    private bool ShouldProcessCommand(string text)
    {
        return _bypassCheck || new Regex(@"\be\s+ai\s+james\b", RegexOptions.IgnoreCase).IsMatch(text);
    }

    private void StartBypassTimer()
    {
        _bypassCheck = true;
        Task.Delay(10000).ContinueWith(_ => _bypassCheck = false);
    }
}