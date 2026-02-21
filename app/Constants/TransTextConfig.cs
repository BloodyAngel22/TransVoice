using app.Core;

namespace app.Constants;

public class TransTextConfig
{
    public static string TextFilePath => GetTextFilePath();

    private static string GetTextFilePath()
    {
        string textFileName = "output.txt";

        string publishPath = Path.Combine(AppContext.BaseDirectory, "..", textFileName);

        if (File.Exists(publishPath))
            return publishPath;

        string devPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", textFileName);

        if (File.Exists(devPath))
            return devPath;

        var settings = UserSettings.LoadOrCreate();
        var language = settings.InterfaceLanguage.ToLower() switch
        {
            "en" => Localizer.Language.English,
            _ => Localizer.Language.Russian,
        };

        string errorMsg =
            $"{Localizer.GetTextWithLanguage(LocalizationKeys.ConfigTextFileNotFound, language)} {publishPath} или {devPath}";
        throw new FileNotFoundException(errorMsg);
    }
}
