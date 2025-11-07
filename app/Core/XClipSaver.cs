using System.Diagnostics;
using app.CLI;

namespace app.Core;

public class XClipSaver
{
    private readonly ConsoleUI _ui = new();

    public async Task SaveToClipboardFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            _ui.Error("Файл не найден.");
            return;
        }

        string args = "-selection clipboard -i";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "xclip",
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            },
        };

        try
        {
            _ui.Info("Копирование в буфер обмена...");
            process.Start();

            string textToCopy = await File.ReadAllTextAsync(filePath);
            await process.StandardInput.WriteAsync(textToCopy);

            process.StandardInput.Close();

            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                _ui.Success("Копирование завершено.");
            }
            else
            {
                string error = await process.StandardError.ReadToEndAsync();
                _ui.Error($"Ошибка xclip (код {process.ExitCode}): {error}");
            }
        }
        catch (Exception ex)
        {
            _ui.Error($"Ошибка запуска xclip: {ex.Message}");
        }
    }
}
