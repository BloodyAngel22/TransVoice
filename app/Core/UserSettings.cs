namespace app.Core;

public class UserSettings
{
    public string? ModelFileName { get; set; }
    public string Language { get; set; } = "auto";
    public string? OpenVinoEncoderFileName { get; set; }
    public bool UseOpenVino { get; set; } = false;
    public string InterfaceLanguage { get; set; } = "ru";

    public static string SettingsFilePath => GetSettingsFilePath();

    private static string GetSettingsFilePath()
    {
        string settingsFileName = "settings.json";

        string publishPath = Path.Combine(AppContext.BaseDirectory, "..", settingsFileName);

        string devPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", settingsFileName);

        return Directory.Exists(Path.GetDirectoryName(publishPath)) ? publishPath : devPath;
    }

    public static UserSettings LoadOrCreate()
    {
        string path = SettingsFilePath;

        if (!File.Exists(path))
        {
            var settings = new UserSettings();
            settings.Save();
            return settings;
        }

        try
        {
            string json = File.ReadAllText(path);
            return System.Text.Json.JsonSerializer.Deserialize<UserSettings>(json)
                ?? new UserSettings();
        }
        catch
        {
            return new UserSettings();
        }
    }

    public void Save()
    {
        string path = SettingsFilePath;
        string? directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        string json = System.Text.Json.JsonSerializer.Serialize(
            this,
            new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
        );

        File.WriteAllText(path, json);
    }
}
