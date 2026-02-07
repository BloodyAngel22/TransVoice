using System.ComponentModel;
using app.Constants;
using app.Core;
using Spectre.Console.Cli;

namespace app.CLI;

public class RecordAndDecodeCommand : Command<RecordAndDecodeCommand.Setting>
{
    public override int Execute(
        CommandContext context,
        Setting settings,
        CancellationToken cancellationToken
    )
    {
        var ui = new ConsoleUI();
        var recorder = new AudioRecorder();
        var converter = new AudioConverter();
        var fixer = new WavFixer();

        ui.ShowHeader();
        string outputFile = AudioConfig.WavFilePath;

        if (!string.IsNullOrWhiteSpace(settings.InputPath))
        {
            string inputPath = settings.InputPath.Trim();

            if (!File.Exists(inputPath))
            {
                ui.Error($"Входной файл не найден: {inputPath}");
                return 1;
            }

            string fullInput = Path.GetFullPath(inputPath);
            string fullOutput = Path.GetFullPath(outputFile);

            ui.Section("Преобразование в WAV");

            if (string.Equals(fullInput, fullOutput, StringComparison.OrdinalIgnoreCase))
            {
                string tempOutput = fullOutput + ".tmp.wav";
                bool converted = converter
                    .ConvertToWav(fullInput, tempOutput)
                    .GetAwaiter()
                    .GetResult();

                if (!converted)
                {
                    if (File.Exists(tempOutput))
                        File.Delete(tempOutput);
                    return 1;
                }

                File.Delete(fullOutput);
                File.Move(tempOutput, fullOutput);
            }
            else
            {
                bool converted = converter
                    .ConvertToWav(fullInput, fullOutput)
                    .GetAwaiter()
                    .GetResult();
                if (!converted)
                    return 1;
            }
        }
        else
        {
            recorder.RecordUntilEnter(outputFile).Wait();
        }

        if (!File.Exists(outputFile))
        {
            ui.Error("Файл записи не найден.");
            return 1;
        }

        ui.Section("Проверка WAV");
        bool corrected = fixer.FixHeader(outputFile);
        if (!corrected)
        {
            ui.Error("Не удалось исправить заголовок WAV.");
            return 1;
        }

        double duration = AudioUtils.GetDuration(outputFile);
        ui.Success($"Файл {Path.GetFileName(outputFile)} готов для Whisper!");

        if (settings.Detail)
        {
            ui.ShowWavInfo(new WavInfo(Path.GetFileName(outputFile), outputFile, duration));
        }

        ui.Section("Распознавание Whisper");
        var start = DateTime.Now;

        string text = ui.RunWithSpinner("Обработка аудио...", WhisperDecoder.Decode).Result;

        FileUtils.WriteToFile(TransTextConfig.TextFilePath, text);

        ui.ShowElapsedTime((DateTime.Now - start).TotalSeconds);

        if (settings.Text)
        {
            ui.ShowResult(text);
        }

        if (settings.Clipboard)
        {
            var xclipSaver = new XClipSaver();
            xclipSaver.SaveToClipboardFromFile(TransTextConfig.TextFilePath).Wait();
        }

        return 0;
    }

    public class Setting : CommandSettings
    {
        [Description("Входной аудио файл для преобразования в WAV")]
        [CommandOption("-i|--input")]
        public string? InputPath { get; init; }

        [Description("Показывать информацию о WAV-файле")]
        [CommandOption("--detail")]
        [DefaultValue(false)]
        public bool Detail { get; init; }

        [Description("Показывать распознанный текст")]
        [CommandOption("--text")]
        [DefaultValue(false)]
        public bool Text { get; init; }

        [Description("Сохранить распознанный текст в буфер обмена")]
        [CommandOption("--clipboard")]
        [DefaultValue(false)]
        public bool Clipboard { get; init; }
    }
}
