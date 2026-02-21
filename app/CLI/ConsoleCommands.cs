using System.ComponentModel;
using System.Text.RegularExpressions;
using app.Constants;
using app.Core;
using Spectre.Console;
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

        var userSettings = UserSettings.LoadOrCreate();
        Localizer.CurrentLanguage = userSettings.InterfaceLanguage.ToLower() switch
        {
            "en" => Localizer.Language.English,
            _ => Localizer.Language.Russian,
        };

        ui.ShowHeader();
        string outputFile = AudioConfig.WavFilePath;

        if (!string.IsNullOrWhiteSpace(settings.InputPath))
        {
            string inputPath = settings.InputPath.Trim();

            if (!File.Exists(inputPath))
            {
                string errorMsg =
                    $"{Localizer.GetText(LocalizationKeys.CommandInputFileNotFound)} {inputPath}";
                ui.Error(errorMsg);
                return 1;
            }

            string fullInput = Path.GetFullPath(inputPath);
            string fullOutput = Path.GetFullPath(outputFile);

            ui.Section(LocalizationKeys.UiSectionConversion);

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
            ui.ErrorKey(LocalizationKeys.CommandRecordingFileNotFound);
            return 1;
        }

        ui.Section(LocalizationKeys.UiSectionWavFix);
        bool corrected = fixer.FixHeader(outputFile);
        if (!corrected)
        {
            ui.ErrorKey(LocalizationKeys.CommandFailedFixWav);
            return 1;
        }

        double duration = AudioUtils.GetDuration(outputFile);
        string fileName = Path.GetFileName(outputFile);
        string successMsg = $"{fileName} {Localizer.GetText(LocalizationKeys.CommandFileReady)}";
        ui.Success(successMsg);

        if (settings.Detail)
        {
            ui.ShowWavInfo(new WavInfo(Path.GetFileName(outputFile), outputFile, duration));
        }

        ui.Section(LocalizationKeys.UiSectionDecoding);
        var start = DateTime.Now;

        string text = ui.RunWithSpinner(
            LocalizationKeys.CommandProcessingAudio,
            WhisperDecoder.Decode
        ).Result;

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
        [Description("Input audio file to convert to WAV")]
        [CommandOption("-i|--input")]
        public string? InputPath { get; init; }

        [Description("Show WAV file information")]
        [CommandOption("--detail")]
        [DefaultValue(false)]
        public bool Detail { get; init; }

        [Description("Show recognized text")]
        [CommandOption("--text")]
        [DefaultValue(false)]
        public bool Text { get; init; }

        [Description("Save recognized text to clipboard")]
        [CommandOption("--clipboard")]
        [DefaultValue(false)]
        public bool Clipboard { get; init; }
    }
}

[Description("Configure application settings")]
public class SettingsCommand : Command<SettingsCommand.Setting>
{
    public override int Execute(
        CommandContext context,
        Setting settings,
        CancellationToken cancellationToken
    )
    {
        var ui = new ConsoleUI();
        var currentSettings = UserSettings.LoadOrCreate();

        Localizer.CurrentLanguage = currentSettings.InterfaceLanguage.ToLower() switch
        {
            "en" => Localizer.Language.English,
            _ => Localizer.Language.Russian,
        };

        ui.ShowHeader();
        ui.Section(LocalizationKeys.SettingsHeader);

        SelectInterfaceLanguage(currentSettings);

        SelectModel(currentSettings, ui);

        SelectRecognitionLanguage(currentSettings);

        SelectOpenVino(currentSettings);

        currentSettings.Save();

        ui.Success(Localizer.GetText(LocalizationKeys.SettingsSaved));
        ui.Info(
            $"{Localizer.GetText(LocalizationKeys.SettingsModel)} {Path.GetFileName(WhisperModelsConfig.ModelFilePath)}"
        );
        ui.Info(
            $"{Localizer.GetText(LocalizationKeys.SettingsLanguage)} {GetLanguageDisplayName(currentSettings.Language)}"
        );

        string openVinoStatus = currentSettings.UseOpenVino
            ? $"OpenVINO: {currentSettings.OpenVinoEncoderFileName ?? Localizer.GetText(LocalizationKeys.SettingsOpenVinoNotSelected)}"
            : $"OpenVINO: {Localizer.GetText(LocalizationKeys.SettingsOpenVinoDisabled)}";
        ui.Info(openVinoStatus);

        return 0;
    }

    private void SelectInterfaceLanguage(UserSettings settings)
    {
        var languages = GetInterfaceLanguages();
        var currentLang = settings.InterfaceLanguage.ToLower() switch
        {
            "en" => Localizer.GetText(LocalizationKeys.InterfaceEnglish),
            _ => Localizer.GetText(LocalizationKeys.InterfaceRussian),
        };

        var langChoices = languages
            .Select(l => l == currentLang ? $"[green]✓[/] {l}" : $"  {l}")
            .ToList();

        var langPrompt = new SelectionPrompt<string>()
            .Title(Localizer.GetText(LocalizationKeys.SettingsSelectInterface))
            .PageSize(5)
            .MoreChoicesText(Localizer.GetText(LocalizationKeys.SettingsUseNavigate))
            .AddChoices(langChoices);

        string selectedLang = AnsiConsole.Prompt(langPrompt);
        selectedLang = CleanSelection(selectedLang);

        settings.InterfaceLanguage = selectedLang switch
        {
            var l when l.Contains("English") || l.Contains("english") => "en",
            _ => "ru",
        };

        Localizer.CurrentLanguage = settings.InterfaceLanguage.ToLower() switch
        {
            "en" => Localizer.Language.English,
            _ => Localizer.Language.Russian,
        };
    }

    private void SelectModel(UserSettings settings, ConsoleUI ui)
    {
        var modelFiles = GetAvailableModels();
        if (modelFiles.Count == 0)
        {
            ui.Error(Localizer.GetText(LocalizationKeys.SettingsNoModels));
            return;
        }

        modelFiles.Insert(0, Localizer.GetText(LocalizationKeys.SettingsSkip));

        var currentModelName = Path.GetFileName(WhisperModelsConfig.ModelFilePath);

        var modelChoices = modelFiles
            .Select(m => m == currentModelName ? $"[green]✓[/] {m}" : $"  {m}")
            .ToList();

        var modelPrompt = new SelectionPrompt<string>()
            .Title(Localizer.GetText(LocalizationKeys.SettingsSelectModel))
            .PageSize(10)
            .MoreChoicesText(Localizer.GetText(LocalizationKeys.SettingsUseNavigate))
            .AddChoices(modelChoices);

        string selectedModel = AnsiConsole.Prompt(modelPrompt);
        selectedModel = CleanSelection(selectedModel);

        if (selectedModel != Localizer.GetText(LocalizationKeys.SettingsSkip))
        {
            settings.ModelFileName = selectedModel;
        }
    }

    private void SelectRecognitionLanguage(UserSettings settings)
    {
        var languages = GetAvailableLanguages();
        languages.Insert(0, Localizer.GetText(LocalizationKeys.SettingsSkip));

        var currentLanguageName = GetLanguageDisplayName(settings.Language);

        var languageChoices = languages
            .Select(l => l == currentLanguageName ? $"[green]✓[/] {l}" : $"  {l}")
            .ToList();

        var languagePrompt = new SelectionPrompt<string>()
            .Title(Localizer.GetText(LocalizationKeys.SettingsSelectLanguage))
            .PageSize(15)
            .MoreChoicesText(Localizer.GetText(LocalizationKeys.SettingsUseNavigate))
            .AddChoices(languageChoices);

        string selectedLanguage = AnsiConsole.Prompt(languagePrompt);
        selectedLanguage = CleanSelection(selectedLanguage);

        if (selectedLanguage != Localizer.GetText(LocalizationKeys.SettingsSkip))
        {
            settings.Language = NormalizeLanguageCode(selectedLanguage);
        }
    }

    private void SelectOpenVino(UserSettings settings)
    {
        var openVinoOptions = GetOpenVinoOptions();
        var currentOpenVinoOption = GetOpenVinoOptionDisplay(settings);

        var openVinoChoices = openVinoOptions
            .Select(o => o == currentOpenVinoOption ? $"[green]✓[/] {o}" : $"  {o}")
            .ToList();

        var openVinoPrompt = new SelectionPrompt<string>()
            .Title(Localizer.GetText(LocalizationKeys.SettingsSelectOpenVino))
            .PageSize(10)
            .MoreChoicesText(Localizer.GetText(LocalizationKeys.SettingsUseNavigate))
            .AddChoices(openVinoChoices);

        string selectedOpenVino = AnsiConsole.Prompt(openVinoPrompt);
        selectedOpenVino = CleanSelection(selectedOpenVino);

        ProcessOpenVinoSelection(selectedOpenVino, settings);
    }

    private List<string> GetAvailableModels()
    {
        var models = new List<string>();

        if (!Directory.Exists(WhisperModelsConfig.ModelFolderName))
            return models;

        foreach (var file in Directory.GetFiles(WhisperModelsConfig.ModelFolderName, "*.bin"))
        {
            models.Add(Path.GetFileName(file));
        }

        return models;
    }

    private List<string> GetAvailableLanguages()
    {
        return
        [
            Localizer.GetText(LocalizationKeys.LangAuto),
            Localizer.GetText(LocalizationKeys.LangRussian),
            Localizer.GetText(LocalizationKeys.LangEnglish),
            Localizer.GetText(LocalizationKeys.LangGerman),
            Localizer.GetText(LocalizationKeys.LangFrench),
            Localizer.GetText(LocalizationKeys.LangSpanish),
            Localizer.GetText(LocalizationKeys.LangItalian),
            Localizer.GetText(LocalizationKeys.LangPortuguese),
            Localizer.GetText(LocalizationKeys.LangChinese),
            Localizer.GetText(LocalizationKeys.LangJapanese),
            Localizer.GetText(LocalizationKeys.LangKorean),
        ];
    }

    private List<string> GetInterfaceLanguages()
    {
        return
        [
            Localizer.GetText(LocalizationKeys.InterfaceRussian),
            Localizer.GetText(LocalizationKeys.InterfaceEnglish),
        ];
    }

    private string NormalizeLanguageCode(string selection)
    {
        return selection.ToLower() switch
        {
            var s when s.Contains("auto") => "auto",
            var s when s.Contains("russian") => "ru",
            var s when s.Contains("english") => "en",
            var s when s.Contains("german") => "de",
            var s when s.Contains("french") => "fr",
            var s when s.Contains("spanish") => "es",
            var s when s.Contains("italian") => "it",
            var s when s.Contains("portuguese") => "pt",
            var s when s.Contains("chinese") => "zh",
            var s when s.Contains("japanese") => "ja",
            var s when s.Contains("korean") => "ko",
            _ => "auto",
        };
    }

    private string GetLanguageDisplayName(string languageCode)
    {
        return languageCode.ToLower() switch
        {
            "auto" => Localizer.GetText(LocalizationKeys.LangAuto),
            "ru" => Localizer.GetText(LocalizationKeys.LangRussian),
            "en" => Localizer.GetText(LocalizationKeys.LangEnglish),
            "de" => Localizer.GetText(LocalizationKeys.LangGerman),
            "fr" => Localizer.GetText(LocalizationKeys.LangFrench),
            "es" => Localizer.GetText(LocalizationKeys.LangSpanish),
            "it" => Localizer.GetText(LocalizationKeys.LangItalian),
            "pt" => Localizer.GetText(LocalizationKeys.LangPortuguese),
            "zh" => Localizer.GetText(LocalizationKeys.LangChinese),
            "ja" => Localizer.GetText(LocalizationKeys.LangJapanese),
            "ko" => Localizer.GetText(LocalizationKeys.LangKorean),
            _ => Localizer.GetText(LocalizationKeys.LangAuto),
        };
    }

    private string CleanSelection(string selection)
    {
        var cleaned = Regex.Replace(selection, @"\[[^\]]+\]", "");

        cleaned = cleaned.TrimStart(' ', '✓');

        return cleaned.Trim();
    }

    private List<string> GetOpenVinoOptions()
    {
        var options = new List<string>
        {
            Localizer.GetText(LocalizationKeys.SettingsSkip),
            Localizer.GetText(LocalizationKeys.SettingsNoOpenVino),
        };

        var xmlFiles = GetAvailableOpenVinoModels();
        foreach (var xmlFile in xmlFiles)
        {
            options.Add($"OpenVINO: {xmlFile}");
        }

        return options;
    }

    private string GetOpenVinoOptionDisplay(UserSettings settings)
    {
        if (!settings.UseOpenVino)
        {
            return Localizer.GetText(LocalizationKeys.SettingsNoOpenVino);
        }

        if (string.IsNullOrEmpty(settings.OpenVinoEncoderFileName))
        {
            return Localizer.GetText(LocalizationKeys.SettingsNoOpenVino);
        }

        return $"OpenVINO: {settings.OpenVinoEncoderFileName}";
    }

    private List<string> GetAvailableOpenVinoModels()
    {
        var models = new List<string>();

        string vinoFolder = WhisperModelsConfig.VinoFolderName;
        if (!Directory.Exists(vinoFolder))
            return models;

        foreach (var file in Directory.GetFiles(vinoFolder, "*.xml"))
        {
            string fileName = Path.GetFileName(file);
            string binFileName = Path.ChangeExtension(fileName, ".bin");

            if (File.Exists(Path.Combine(vinoFolder, binFileName)))
            {
                models.Add(fileName);
            }
        }

        return models;
    }

    private void ProcessOpenVinoSelection(string selection, UserSettings settings)
    {
        string skip = Localizer.GetText(LocalizationKeys.SettingsSkip);
        string noOpenVino = Localizer.GetText(LocalizationKeys.SettingsNoOpenVino);

        if (selection == skip)
        {
            return;
        }

        if (selection == noOpenVino)
        {
            settings.UseOpenVino = false;
            settings.OpenVinoEncoderFileName = null;
            return;
        }

        if (selection.StartsWith("OpenVINO: "))
        {
            string fileName = selection.Substring("OpenVINO: ".Length);
            settings.UseOpenVino = true;
            settings.OpenVinoEncoderFileName = fileName;
        }
    }

    public class Setting : CommandSettings { }
}
