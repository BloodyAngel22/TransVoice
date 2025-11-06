using System.ComponentModel;
using AudioListener.Constants;
using AudioListener.Core;
using Spectre.Console.Cli;

namespace AudioListener.CLI;

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
        var fixer = new WavFixer();

        ui.ShowHeader();
        string outputFile = AudioConfig.WavFilePath;
        recorder.RecordUntilEnter(outputFile).Wait();

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

        ui.Info($"Распознанные данные записаны в output.txt");

        return 0;
    }

    public class Setting : CommandSettings
    {
        [Description("Показывать информацию о WAV-файле")]
        [CommandOption("--detail")]
        [DefaultValue(false)]
        public bool Detail { get; init; }

        [Description("Показывать распознанный текст")]
        [CommandOption("--text")]
        [DefaultValue(false)]
        public bool Text { get; init; }
    }
}
