namespace AudioListener.Core;

public class FileUtils
{
    public static void WriteToFile(string path, string content)
    {
        File.WriteAllText(path, content);
    }
}
