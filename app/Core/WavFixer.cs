using app.CLI;
using app.Constants;
using NAudio.Wave;

namespace app.Core;

public class WavFixer
{
    private readonly ConsoleUI _ui = new();

    public bool FixHeader(string path)
    {
        try
        {
            using var reader = new WaveFileReader(path);

            if (
                reader.WaveFormat.SampleRate != AudioConfig.SampleRate
                || reader.WaveFormat.BitsPerSample != AudioConfig.BitDepth
                || reader.WaveFormat.Channels != AudioConfig.Channels
            )
            {
                _ui.Info("Обнаружен неверный формат аудио — выполняется пересоздание заголовка.");
            }

            string tempPath = path + ".fixed";
            WaveFileWriter.CreateWaveFile(tempPath, reader);
            reader.Dispose();

            File.Delete(path);
            File.Move(tempPath, path);

            return true;
        }
        catch (Exception ex)
        {
            _ui.Error($"Ошибка исправления WAV-заголовка: {ex.Message}");
            return false;
        }
    }
}
