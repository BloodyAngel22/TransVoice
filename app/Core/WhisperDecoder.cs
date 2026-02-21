using System.Text;
using app.Constants;
using Whisper.net;

namespace app.Core;

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

        var settings = UserSettings.LoadOrCreate();

        var builder = whisperFactory
            .CreateBuilder()
            .WithThreads(Environment.ProcessorCount / 2)
            .WithTemperature(0.0f)
            .WithMaxTokensPerSegment(256)
            .WithNoSpeechThreshold(0.7f)
            .WithEntropyThreshold(2.0f);

        if (settings.Language != "auto")
        {
            builder = builder.WithLanguage(settings.Language);
        }

        bool useOpenVino =
            settings.UseOpenVino
            && !string.IsNullOrEmpty(WhisperModelsConfig.VinoEncoderFilePath)
            && File.Exists(WhisperModelsConfig.VinoEncoderFilePath);

        if (useOpenVino)
        {
            builder = builder.WithOpenVinoEncoder(
                openVinoEncoderPath: WhisperModelsConfig.VinoEncoderFilePath,
                openVinoDevice: "GPU",
                openVinoCachePath: "ov_cache"
            );
        }

        using var processor = builder.WithGreedySamplingStrategy().ParentBuilder.Build();

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
