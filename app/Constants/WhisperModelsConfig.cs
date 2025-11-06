namespace AudioListener.Constants;

public class WhisperModelsConfig
{
    public static string ModelFilePath => GetModelPath();

    private static string GetModelPath()
    {
        string modelFileName = "ggml-large-v3-turbo-q8_0.bin";
        string modelFolderName = "Models";

        string publishPath = Path.Combine(
            AppContext.BaseDirectory,
            "..",
            modelFolderName,
            modelFileName
        );

        if (File.Exists(publishPath))
            return publishPath;

        string devPath = Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            modelFolderName,
            modelFileName
        );

        if (File.Exists(devPath))
            return devPath;

        throw new FileNotFoundException($"Модель не найдена по пути: {publishPath} или {devPath}");
    }
}
