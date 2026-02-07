using System.Diagnostics;
using app.CLI;
using app.Constants;

namespace app.Core;

public class AudioConverter
{
    private readonly ConsoleUI _ui = new();

    public async Task<bool> ConvertToWav(string inputPath, string outputPath)
    {
        if (File.Exists(outputPath))
            File.Delete(outputPath);

        string args =
            $"-y -i \"{inputPath}\" -ac {AudioConfig.Channels} -ar {AudioConfig.SampleRate} -sample_fmt s16 \"{outputPath}\"";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            },
        };

        try
        {
            _ui.Info("Преобразование через ffmpeg...");
            process.Start();

            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                _ui.Error($"Ошибка ffmpeg (код {process.ExitCode}): {error}");
                return false;
            }

            if (!File.Exists(outputPath))
            {
                _ui.Error("Не удалось создать WAV-файл.");
                return false;
            }

            _ui.Success("Преобразование завершено.");
            return true;
        }
        catch (Exception ex)
        {
            _ui.Error($"Ошибка запуска ffmpeg: {ex.Message}");
            return false;
        }
    }
}
