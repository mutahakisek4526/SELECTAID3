using System;
using System.Speech.Synthesis;

namespace SelectAid.Services;

public sealed class SpeechService
{
    private readonly SpeechSynthesizer _synthesizer = new();

    public SpeechService()
    {
        _synthesizer.SetOutputToDefaultAudioDevice();
    }

    public void Speak(string text)
    {
        try
        {
            _synthesizer.SpeakAsyncCancelAll();
            _synthesizer.SpeakAsync(text);
        }
        catch (Exception ex)
        {
            LoggingService.LogError("Speech failed", ex);
        }
    }
}
