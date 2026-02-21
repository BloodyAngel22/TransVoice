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
            _ui.InfoKey(LocalizationKeys.ConverterConverting);
            process.Start();

            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                string errorMsg =
                    $"{Localizer.GetText(LocalizationKeys.ConverterFfmpegError)} {process.ExitCode}: {error}";
                _ui.Error(errorMsg);
                return false;
            }

            if (!File.Exists(outputPath))
            {
                _ui.ErrorKey(LocalizationKeys.ConverterWavNotCreated);
                return false;
            }

            _ui.SuccessKey(LocalizationKeys.ConverterConversionComplete);
            return true;
        }
        catch (Exception ex)
        {
            string errorMsg =
                $"{Localizer.GetText(LocalizationKeys.ConverterFfmpegStartError)} {ex.Message}";
            _ui.Error(errorMsg);
            return false;
        }
    }
}
