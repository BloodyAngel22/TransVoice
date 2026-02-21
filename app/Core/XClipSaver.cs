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
            _ui.ErrorKey(LocalizationKeys.XClipFileNotFound);
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
            _ui.InfoKey(LocalizationKeys.XClipCopying);
            process.Start();

            string textToCopy = await File.ReadAllTextAsync(filePath);
            await process.StandardInput.WriteAsync(textToCopy);

            process.StandardInput.Close();

            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                _ui.SuccessKey(LocalizationKeys.XClipCompleted);
            }
            else
            {
                string error = await process.StandardError.ReadToEndAsync();
                string errorMsg =
                    $"{Localizer.GetText(LocalizationKeys.XClipError)} {process.ExitCode}: {error}";
                _ui.Error(errorMsg);
            }
        }
        catch (Exception ex)
        {
            string errorMsg = $"{Localizer.GetText(LocalizationKeys.XClipStartError)} {ex.Message}";
            _ui.Error(errorMsg);
        }
    }
}
