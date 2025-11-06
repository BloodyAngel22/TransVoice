using System.Diagnostics;
using AudioListener.CLI;
using AudioListener.Constants;

namespace AudioListener.Core;

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
                _ui.Error("Файл записи слишком мал или не создан.");
            else
                _ui.Success("Запись завершена успешно.");
        }
        catch (Exception ex)
        {
            _ui.Error($"Ошибка записи: {ex.Message}");
            if (!process.HasExited)
                process.Kill();
        }
    }
}
