namespace app.Constants;

public static class AudioConfig
{
    public const int SampleRate = 16000;
    public const int BitDepth = 16;
    public const int Channels = 1;

    public static string WavFilePath => GetWavPath();

    private static string GetWavPath()
    {
        string wavFileName = "whisper_recording.wav";

        return Path.Combine(AppContext.BaseDirectory, wavFileName);
    }
}
