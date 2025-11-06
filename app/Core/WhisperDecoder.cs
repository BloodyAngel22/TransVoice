using System.Text;
using AudioListener.Constants;
using Whisper.net;

namespace AudioListener.Core;

public class WhisperDecoder
{
    public static async Task<string> Decode()
    {
        if (!File.Exists(WhisperModelsConfig.ModelFilePath))
        {
            throw new FileNotFoundException(
                $"Модель не найдена по пути: {WhisperModelsConfig.ModelFilePath}"
            );
        }

        using var whisperFactory = WhisperFactory.FromPath(WhisperModelsConfig.ModelFilePath);

        using var processor = whisperFactory.CreateBuilder().WithLanguage("ru").Build();

        StringBuilder text = new();

        using var fileStream = new FileStream(AudioConfig.WavFilePath, FileMode.Open);

        await foreach (var result in processor.ProcessAsync(fileStream))
        {
            text.Append(result.Text);
        }

        if (text.Length != 0 && text[0] == ' ')
            text.Remove(0, 1);

        return text.ToString();
    }
}
