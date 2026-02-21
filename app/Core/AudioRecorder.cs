using System.Diagnostics;
using app.CLI;
using app.Constants;

namespace app.Core;

public class AudioRecorder
{
    private readonly ConsoleUI _ui = new();

    public async Task RecordUntilEnter(string outputPath)
    {
        if (File.Exists(outputPath))
            File.Delete(outputPath);

        string args =
            $"-f S16_LE -r {AudioConfig.SampleRate} -c {AudioConfig.Channels} {outputPath}";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "arecord",
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            },
        };

        try
        {
            _ui.WaitForStart();
            process.Start();
            _ui.WaitForStop();

            if (!process.HasExited)
                process.Kill();

            await process.WaitForExitAsync();

            if (!File.Exists(outputPath) || new FileInfo(outputPath).Length < 44)
                _ui.ErrorKey(LocalizationKeys.RecorderFileTooSmall);
            else
                _ui.SuccessKey(LocalizationKeys.RecorderCompleted);
        }
        catch (Exception ex)
        {
            string errorMsg = $"{Localizer.GetText(LocalizationKeys.RecorderError)} {ex.Message}";
            _ui.Error(errorMsg);
            if (!process.HasExited)
                process.Kill();
        }
    }
}
