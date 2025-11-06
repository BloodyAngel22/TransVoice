using NAudio.Wave;

namespace AudioListener.Core;

public static class AudioUtils
{
    public static double GetDuration(string filePath)
    {
        try
        {
            using var reader = new WaveFileReader(filePath);
            return reader.TotalTime.TotalSeconds;
        }
        catch
        {
            return 0.0;
        }
    }
}
