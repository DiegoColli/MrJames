using Microsoft.CognitiveServices.Speech;
using MrDucky;
using OpenAI.ChatGpt;


var config = SpeechConfig.FromSubscription("mssecret", "brazilsouth");
config.SpeechSynthesisVoiceName = "pt-BR-FranciscaNeural";
config.SpeechRecognitionLanguage = "pt-BR";

var client = new OpenAiClient("gptsecret");

await using var speechHelper = new SpeechHelper(client, new SpeechSynthesizer(config), new SpeechRecognizer(config));
await speechHelper.InitializeAsync();

Console.WriteLine("Pressione qualquer tecla para finalizar...");
Console.ReadKey();
