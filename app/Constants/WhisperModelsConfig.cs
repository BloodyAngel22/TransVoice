using app.Core;

namespace app.Constants;

public class WhisperModelsConfig
{
    public static string ModelFilePath => GetModelFilePath();
    public static string ModelFolderName => GetModelFolderPath();
    public static string VinoEncoderFilePath => GetVinoEncoderFilePath();
    public static string VinoFolderName => GetVinoFolderPath();

    private static string GetModelFolderPath()
    {
        string modelFolderName = "Models";

        string publishPath = Path.Combine(AppContext.BaseDirectory, "..", modelFolderName);

        if (Directory.Exists(publishPath))
            return publishPath;

        string devPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", modelFolderName);

        if (Directory.Exists(devPath))
            return devPath;

        var settings = UserSettings.LoadOrCreate();
        var language = settings.InterfaceLanguage.ToLower() switch
        {
            "en" => Localizer.Language.English,
            _ => Localizer.Language.Russian,
        };

        string errorMsg =
            $"{Localizer.GetTextWithLanguage(LocalizationKeys.ConfigModelNotFound, language)} {publishPath} или {devPath}";
        throw new FileNotFoundException(errorMsg);
    }

    private static string GetModelFilePath()
    {
        var settings = UserSettings.LoadOrCreate();

        string modelFileName = settings.ModelFileName ?? "ggml-large-v3-turbo-q8_0.bin";

        string modelPath = Path.Combine(GetModelFolderPath(), modelFileName);

        if (!File.Exists(modelPath))
        {
            modelFileName = "ggml-large-v3-turbo-q8_0.bin";
            modelPath = Path.Combine(GetModelFolderPath(), modelFileName);
        }

        return modelPath;
    }

    private static string GetVinoEncoderFilePath()
    {
        var settings = UserSettings.LoadOrCreate();

        if (!settings.UseOpenVino || string.IsNullOrEmpty(settings.OpenVinoEncoderFileName))
        {
            return string.Empty;
        }

        string modelFileName = settings.OpenVinoEncoderFileName;
        return Path.Combine(GetVinoFolderPath(), modelFileName);
    }

    private static string GetVinoFolderPath()
    {
        string vinoFolderName = "OpenVinoModels";

        string publishPath = Path.Combine(AppContext.BaseDirectory, "..", vinoFolderName);

        if (Directory.Exists(publishPath))
            return publishPath;

        string devPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", vinoFolderName);

        if (Directory.Exists(devPath))
            return devPath;

        return publishPath;
    }
}
