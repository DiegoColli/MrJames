namespace MrJames;

class JamesService(SpeechSynthesisHandler synthesizer, SpeechRecognitionHandler recognizer) : IAsyncDisposable
{
    public async Task InitializeAsync() => await recognizer.StartAsync();

    public async ValueTask DisposeAsync()
    {
        await recognizer.StopAsync();
        await DisposeIfAsyncDisposable(synthesizer);
        await DisposeIfAsyncDisposable(synthesizer);

        static async ValueTask DisposeIfAsyncDisposable(object obj)
        {
            if (obj is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
            else if (obj is IDisposable disposable)
                disposable.Dispose();
        }
    }
}