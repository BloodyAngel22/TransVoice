namespace app.Core;

public static class LocalizationKeys
{
    // App
    public const string AppName = "app.name";
    public const string AppDescription = "app.description";

    // ConsoleUI
    public const string UiHeader = "ui.header";
    public const string UiRecording = "ui.recording";
    public const string UiConverting = "ui.converting";
    public const string UiFixingWav = "ui.fixing_wav";
    public const string UiDecoding = "ui.decoding";
    public const string UiSaving = "ui.saving";
    public const string UiCopying = "ui.copying";
    public const string UiDone = "ui.done";
    public const string UiSection = "ui.section";
    public const string UiSectionConversion = "ui.section_conversion";
    public const string UiSectionWavFix = "ui.section_wav_fix";
    public const string UiSectionDecoding = "ui.section_decoding";
    public const string UiSuccess = "ui.success";
    public const string UiError = "ui.error";
    public const string UiInfo = "ui.info";
    public const string UiWarning = "ui.warning";

    // Settings
    public const string SettingsHeader = "settings.header";
    public const string SettingsSelectModel = "settings.select_model";
    public const string SettingsSelectLanguage = "settings.select_language";
    public const string SettingsSelectOpenVino = "settings.select_openvino";
    public const string SettingsSelectInterface = "settings.select_interface";
    public const string SettingsSkip = "settings.skip";
    public const string SettingsNoOpenVino = "settings.no_openvino";
    public const string SettingsNoModels = "settings.no_models";
    public const string SettingsSaved = "settings.saved";
    public const string SettingsModel = "settings.model";
    public const string SettingsLanguage = "settings.language";
    public const string SettingsOpenVino = "settings.openvino";
    public const string SettingsOpenVinoDisabled = "settings.openvino_disabled";
    public const string SettingsOpenVinoNotSelected = "settings.openvino_not_selected";
    public const string SettingsUseNavigate = "settings.use_navigate";

    // Languages
    public const string LangAuto = "lang.auto";
    public const string LangRussian = "lang.russian";
    public const string LangEnglish = "lang.english";
    public const string LangGerman = "lang.german";
    public const string LangFrench = "lang.french";
    public const string LangSpanish = "lang.spanish";
    public const string LangItalian = "lang.italian";
    public const string LangPortuguese = "lang.portuguese";
    public const string LangChinese = "lang.chinese";
    public const string LangJapanese = "lang.japanese";
    public const string LangKorean = "lang.korean";

    // Interface languages
    public const string InterfaceRussian = "interface.russian";
    public const string InterfaceEnglish = "interface.english";

    // AudioConverter
    public const string ConverterConverting = "converter.converting";
    public const string ConverterFfmpegError = "converter.ffmpeg_error";
    public const string ConverterWavNotCreated = "converter.wav_not_created";
    public const string ConverterConversionComplete = "converter.conversion_complete";
    public const string ConverterFfmpegStartError = "converter.ffmpeg_start_error";

    // WavFixer
    public const string WavFixerWrongFormat = "wavfixer.wrong_format";
    public const string WavFixerError = "wavfixer.error";

    // AudioRecorder
    public const string RecorderFileTooSmall = "recorder.file_too_small";
    public const string RecorderCompleted = "recorder.completed";
    public const string RecorderError = "recorder.error";

    // XClipSaver
    public const string XClipFileNotFound = "xclip.file_not_found";
    public const string XClipCopying = "xclip.copying";
    public const string XClipCompleted = "xclip.completed";
    public const string XClipError = "xclip.error";
    public const string XClipStartError = "xclip.start_error";

    // RecordAndDecodeCommand
    public const string CommandInputFileNotFound = "command.input_file_not_found";
    public const string CommandRecordingFileNotFound = "command.recording_file_not_found";
    public const string CommandFailedFixWav = "command.failed_fix_wav";
    public const string CommandFileReady = "command.file_ready";
    public const string CommandProcessingAudio = "command.processing_audio";

    // Config
    public const string ConfigModelNotFound = "config.model_not_found";
    public const string ConfigTextFileNotFound = "config.text_file_not_found";
}
