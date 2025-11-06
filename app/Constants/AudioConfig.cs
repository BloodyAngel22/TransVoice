namespace AudioListener.Constants;

public static class AudioConfig
{
    public const int SampleRate = 16000;
    public const int BitDepth = 16;
    public const int Channels = 1;

    public static string WavFilePath => GetWavPath();

    private static string GetWavPath()
    {
        string wavFileName = "whisper_recording.wav";

        string publishPath = Path.Combine(AppContext.BaseDirectory, "..", wavFileName);

        if (File.Exists(publishPath))
            return publishPath;

        string devPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", wavFileName);

        if (File.Exists(devPath))
            return devPath;

        throw new FileNotFoundException(
            $"Файл записи не найден по пути: {publishPath} или {devPath}"
        );
    }
}
