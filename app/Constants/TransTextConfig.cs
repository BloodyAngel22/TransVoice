namespace AudioListener.Constants;

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

        throw new FileNotFoundException(
            $"Файл текста не найден по пути: {publishPath} или {devPath}"
        );
    }
}
