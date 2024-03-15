using Microsoft.CognitiveServices.Speech;
using MrJames;
using OpenAI.ChatGpt;


var config = SpeechConfig.FromSubscription("mssecret", "brazilsouth");
config.SpeechSynthesisVoiceName = "pt-BR-FranciscaNeural";
config.SpeechRecognitionLanguage = "pt-BR";

var client = new OpenAiClient("gptsecret");

var speechSynthesizer = new SpeechSynthesizer(config);
var speechRecognizer = new SpeechRecognizer(config);

var synthesisHandler = new SpeechSynthesisHandler(speechSynthesizer);
var commandHandler = new CommandHandler(client, synthesisHandler);
var recognitionHandler = new SpeechRecognitionHandler(speechRecognizer, commandHandler.ProcessCommandAsync);

await using var speechHelper = new JamesService(synthesisHandler, recognitionHandler);


await speechHelper.InitializeAsync();

Console.WriteLine("Pressione qualquer tecla para finalizar...");
Console.ReadKey();
