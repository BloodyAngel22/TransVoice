namespace app.Core;

public static class Localizer
{
    public enum Language
    {
        Russian,
        English,
    }

    private static Language _currentLanguage = Language.Russian;

    public static Language CurrentLanguage
    {
        get => _currentLanguage;
        set => _currentLanguage = value;
    }

    public static string GetText(string key)
    {
        return _currentLanguage switch
        {
            Language.English => GetEnglishText(key),
            _ => GetRussianText(key),
        };
    }

    public static string GetTextWithLanguage(string key, Language language)
    {
        return language switch
        {
            Language.English => GetEnglishText(key),
            _ => GetRussianText(key),
        };
    }

    private static string GetRussianText(string key)
    {
        return key switch
        {
            LocalizationKeys.AppName => "TransVoice",
            LocalizationKeys.AppDescription => "Распознавание речи с помощью модели Whisper",

            // ConsoleUI
            LocalizationKeys.UiHeader => "🎤 TransVoice — Распознавание речи",
            LocalizationKeys.UiRecording => "📝 Запись...",
            LocalizationKeys.UiConverting => "🔄 Конвертация аудио...",
            LocalizationKeys.UiFixingWav => "🔧 Исправление WAV заголовка...",
            LocalizationKeys.UiDecoding => "🧠 Декодирование...",
            LocalizationKeys.UiSaving => "💾 Сохранение текста...",
            LocalizationKeys.UiCopying => "📋 Копирование в буфер обмена...",
            LocalizationKeys.UiDone => "✅ Готово!",
            LocalizationKeys.UiSection => "Настройки приложения",
            LocalizationKeys.UiSectionConversion => "Конвертация аудио",
            LocalizationKeys.UiSectionWavFix => "Проверка WAV",
            LocalizationKeys.UiSectionDecoding => "Распознавание Whisper",
            LocalizationKeys.UiSuccess => "✓ Успешно",
            LocalizationKeys.UiError => "✗ Ошибка",
            LocalizationKeys.UiInfo => "ℹ️ Информация",
            LocalizationKeys.UiWarning => "⚠️ Предупреждение",

            // Settings
            LocalizationKeys.SettingsHeader => "Настройки приложения",
            LocalizationKeys.SettingsSelectModel => "Выберите модель Whisper:",
            LocalizationKeys.SettingsSelectLanguage => "Выберите язык распознавания:",
            LocalizationKeys.SettingsSelectOpenVino => "Выберите использование OpenVINO:",
            LocalizationKeys.SettingsSelectInterface => "Выберите язык интерфейса:",
            LocalizationKeys.SettingsSkip => "Пропустить (оставить как есть)",
            LocalizationKeys.SettingsNoOpenVino => "Не использовать OpenVINO",
            LocalizationKeys.SettingsNoModels => "Модели не найдены в папке Models",
            LocalizationKeys.SettingsSaved => "Настройки сохранены!",
            LocalizationKeys.SettingsModel => "Модель:",
            LocalizationKeys.SettingsLanguage => "Язык:",
            LocalizationKeys.SettingsOpenVino => "OpenVINO:",
            LocalizationKeys.SettingsOpenVinoDisabled => "отключено",
            LocalizationKeys.SettingsOpenVinoNotSelected => "не выбрана",
            LocalizationKeys.SettingsUseNavigate => "[grey](Используйте стрелки для навигации)[/]",

            // Languages
            LocalizationKeys.LangAuto => "Auto (автоопределение)",
            LocalizationKeys.LangRussian => "Russian (русский)",
            LocalizationKeys.LangEnglish => "English (английский)",
            LocalizationKeys.LangGerman => "German (немецкий)",
            LocalizationKeys.LangFrench => "French (французский)",
            LocalizationKeys.LangSpanish => "Spanish (испанский)",
            LocalizationKeys.LangItalian => "Italian (итальянский)",
            LocalizationKeys.LangPortuguese => "Portuguese (португальский)",
            LocalizationKeys.LangChinese => "Chinese (китайский)",
            LocalizationKeys.LangJapanese => "Japanese (японский)",
            LocalizationKeys.LangKorean => "Korean (корейский)",

            // Interface languages
            LocalizationKeys.InterfaceRussian => "Русский",
            LocalizationKeys.InterfaceEnglish => "English",

            // AudioConverter
            LocalizationKeys.ConverterConverting => "Преобразование через ffmpeg...",
            LocalizationKeys.ConverterFfmpegError => "Ошибка ffmpeg (код",
            LocalizationKeys.ConverterWavNotCreated => "Не удалось создать WAV-файл.",
            LocalizationKeys.ConverterConversionComplete => "Преобразование завершено.",
            LocalizationKeys.ConverterFfmpegStartError => "Ошибка запуска ffmpeg:",

            // WavFixer
            LocalizationKeys.WavFixerWrongFormat =>
                "Обнаружен неверный формат аудио — выполняется пересоздание заголовка.",
            LocalizationKeys.WavFixerError => "Ошибка исправления WAV-заголовка:",

            // AudioRecorder
            LocalizationKeys.RecorderFileTooSmall => "Файл записи слишком мал или не создан.",
            LocalizationKeys.RecorderCompleted => "Запись завершена успешно.",
            LocalizationKeys.RecorderError => "Ошибка записи:",

            // XClipSaver
            LocalizationKeys.XClipFileNotFound => "Файл не найден.",
            LocalizationKeys.XClipCopying => "Копирование в буфер обмена...",
            LocalizationKeys.XClipCompleted => "Копирование завершено.",
            LocalizationKeys.XClipError => "Ошибка xclip (код",
            LocalizationKeys.XClipStartError => "Ошибка запуска xclip:",

            // RecordAndDecodeCommand
            LocalizationKeys.CommandInputFileNotFound => "Входной файл не найден:",
            LocalizationKeys.CommandRecordingFileNotFound => "Файл записи не найден.",
            LocalizationKeys.CommandFailedFixWav => "Не удалось исправить заголовок WAV.",
            LocalizationKeys.CommandFileReady => "готов для Whisper!",
            LocalizationKeys.CommandProcessingAudio => "Обработка аудио...",

            // Config
            LocalizationKeys.ConfigModelNotFound => "Модель не найдена по пути:",
            LocalizationKeys.ConfigTextFileNotFound => "Файл текста не найден по пути:",

            _ => key,
        };
    }

    private static string GetEnglishText(string key)
    {
        return key switch
        {
            LocalizationKeys.AppName => "TransVoice",
            LocalizationKeys.AppDescription => "Speech recognition using Whisper model",

            // ConsoleUI
            LocalizationKeys.UiHeader => "🎤 TransVoice — Speech Recognition",
            LocalizationKeys.UiRecording => "📝 Recording...",
            LocalizationKeys.UiConverting => "🔄 Converting audio...",
            LocalizationKeys.UiFixingWav => "🔧 Fixing WAV header...",
            LocalizationKeys.UiDecoding => "🧠 Decoding...",
            LocalizationKeys.UiSaving => "💾 Saving text...",
            LocalizationKeys.UiCopying => "📋 Copying to clipboard...",
            LocalizationKeys.UiDone => "✅ Done!",
            LocalizationKeys.UiSection => "Application Settings",
            LocalizationKeys.UiSectionConversion => "Converting audio",
            LocalizationKeys.UiSectionWavFix => "Checking WAV",
            LocalizationKeys.UiSectionDecoding => "Whisper recognition",
            LocalizationKeys.UiSuccess => "✓ Success",
            LocalizationKeys.UiError => "✗ Error",
            LocalizationKeys.UiInfo => "ℹ️ Info",
            LocalizationKeys.UiWarning => "⚠️ Warning",

            // Settings
            LocalizationKeys.SettingsHeader => "Application Settings",
            LocalizationKeys.SettingsSelectModel => "Select Whisper model:",
            LocalizationKeys.SettingsSelectLanguage => "Select recognition language:",
            LocalizationKeys.SettingsSelectOpenVino => "Select OpenVINO usage:",
            LocalizationKeys.SettingsSelectInterface => "Select interface language:",
            LocalizationKeys.SettingsSkip => "Skip (keep current)",
            LocalizationKeys.SettingsNoOpenVino => "Don't use OpenVINO",
            LocalizationKeys.SettingsNoModels => "No models found in Models folder",
            LocalizationKeys.SettingsSaved => "Settings saved!",
            LocalizationKeys.SettingsModel => "Model:",
            LocalizationKeys.SettingsLanguage => "Language:",
            LocalizationKeys.SettingsOpenVino => "OpenVINO:",
            LocalizationKeys.SettingsOpenVinoDisabled => "disabled",
            LocalizationKeys.SettingsOpenVinoNotSelected => "not selected",
            LocalizationKeys.SettingsUseNavigate => "[grey](Use arrow keys to navigate)[/]",

            // Languages
            LocalizationKeys.LangAuto => "Auto (auto-detect)",
            LocalizationKeys.LangRussian => "Russian",
            LocalizationKeys.LangEnglish => "English",
            LocalizationKeys.LangGerman => "German",
            LocalizationKeys.LangFrench => "French",
            LocalizationKeys.LangSpanish => "Spanish",
            LocalizationKeys.LangItalian => "Italian",
            LocalizationKeys.LangPortuguese => "Portuguese",
            LocalizationKeys.LangChinese => "Chinese",
            LocalizationKeys.LangJapanese => "Japanese",
            LocalizationKeys.LangKorean => "Korean",

            // Interface languages
            LocalizationKeys.InterfaceRussian => "Русский",
            LocalizationKeys.InterfaceEnglish => "English",

            // AudioConverter
            LocalizationKeys.ConverterConverting => "Converting via ffmpeg...",
            LocalizationKeys.ConverterFfmpegError => "ffmpeg error (code",
            LocalizationKeys.ConverterWavNotCreated => "Failed to create WAV file.",
            LocalizationKeys.ConverterConversionComplete => "Conversion complete.",
            LocalizationKeys.ConverterFfmpegStartError => "Error starting ffmpeg:",

            // WavFixer
            LocalizationKeys.WavFixerWrongFormat =>
                "Invalid audio format detected — recreating WAV header.",
            LocalizationKeys.WavFixerError => "Error fixing WAV header:",

            // AudioRecorder
            LocalizationKeys.RecorderFileTooSmall => "Recording file too small or not created.",
            LocalizationKeys.RecorderCompleted => "Recording completed successfully.",
            LocalizationKeys.RecorderError => "Recording error:",

            // XClipSaver
            LocalizationKeys.XClipFileNotFound => "File not found.",
            LocalizationKeys.XClipCopying => "Copying to clipboard...",
            LocalizationKeys.XClipCompleted => "Copy completed.",
            LocalizationKeys.XClipError => "xclip error (code",
            LocalizationKeys.XClipStartError => "Error starting xclip:",

            // RecordAndDecodeCommand
            LocalizationKeys.CommandInputFileNotFound => "Input file not found:",
            LocalizationKeys.CommandRecordingFileNotFound => "Recording file not found.",
            LocalizationKeys.CommandFailedFixWav => "Failed to fix WAV header.",
            LocalizationKeys.CommandFileReady => "is ready for Whisper!",
            LocalizationKeys.CommandProcessingAudio => "Processing audio...",

            // Config
            LocalizationKeys.ConfigModelNotFound => "Model not found at path:",
            LocalizationKeys.ConfigTextFileNotFound => "Text file not found at path:",

            _ => key,
        };
    }
}
