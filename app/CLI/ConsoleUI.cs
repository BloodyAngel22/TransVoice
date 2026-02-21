using app.Core;
using Spectre.Console;

namespace app.CLI;

public class ConsoleUI
{
    public void ShowHeader()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("TransVoice").Color(Color.Cyan1).Centered());
    }

    public void Section(string? key = null)
    {
        string title =
            key != null ? Localizer.GetText(key) : Localizer.GetText(LocalizationKeys.UiSection);
        AnsiConsole.Write(new Rule($"[yellow]{title}[/]").LeftJustified());
    }

    public void Info(string message)
    {
        string prefix = Localizer.GetText(LocalizationKeys.UiInfo);
        AnsiConsole.MarkupLine($"[blue]{prefix}[/] {message}");
    }

    public void InfoKey(string key)
    {
        string prefix = Localizer.GetText(LocalizationKeys.UiInfo);
        string message = Localizer.GetText(key);
        AnsiConsole.MarkupLine($"[blue]{prefix}[/] {message}");
    }

    public void Success(string message)
    {
        string prefix = Localizer.GetText(LocalizationKeys.UiSuccess);
        AnsiConsole.MarkupLine($"[green]{prefix}[/] {message}");
    }

    public void SuccessKey(string key)
    {
        string prefix = Localizer.GetText(LocalizationKeys.UiSuccess);
        string message = Localizer.GetText(key);
        AnsiConsole.MarkupLine($"[green]{prefix}[/] {message}");
    }

    public void Error(string message)
    {
        string prefix = Localizer.GetText(LocalizationKeys.UiError);
        AnsiConsole.MarkupLine($"[red]{prefix}[/] {message}");
    }

    public void ErrorKey(string key)
    {
        string prefix = Localizer.GetText(LocalizationKeys.UiError);
        string message = Localizer.GetText(key);
        AnsiConsole.MarkupLine($"[red]{prefix}[/] {message}");
    }

    public void Warning(string message)
    {
        string prefix = Localizer.GetText(LocalizationKeys.UiWarning);
        AnsiConsole.MarkupLine($"[yellow]{prefix}[/] {message}");
    }

    public void Detail(string message)
    {
        AnsiConsole.MarkupLine($"[grey]•[/] {message}");
    }

    public async Task<string> RunWithSpinner(string titleKey, Func<Task<string>> action)
    {
        string title = Localizer.GetText(titleKey);
        string result = string.Empty;
        await AnsiConsole
            .Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("green"))
            .StartAsync(
                title,
                async ctx =>
                {
                    result = await action();
                }
            );
        return result;
    }

    public void ShowWavInfo(WavInfo info)
    {
        var table = new Table().Border(TableBorder.Rounded);
        string propertyCol =
            Localizer.CurrentLanguage == Localizer.Language.English ? "Property" : "Свойство";
        string valueCol =
            Localizer.CurrentLanguage == Localizer.Language.English ? "Value" : "Значение";
        string fileLabel =
            Localizer.CurrentLanguage == Localizer.Language.English ? "File" : "Файл";
        string pathLabel =
            Localizer.CurrentLanguage == Localizer.Language.English ? "Path" : "Путь";
        string durationLabel =
            Localizer.CurrentLanguage == Localizer.Language.English ? "Duration" : "Длительность";
        string secLabel = Localizer.CurrentLanguage == Localizer.Language.English ? "sec." : "сек.";

        table.AddColumn($"[cyan]{propertyCol}[/]");
        table.AddColumn($"[green]{valueCol}[/]");
        table.AddRow(fileLabel, info.FileName);
        table.AddRow(pathLabel, info.FilePath);
        table.AddRow(durationLabel, $"{info.Duration:F2} {secLabel}");

        AnsiConsole.Write(table);
    }

    public void ShowElapsedTime(double elapsedSeconds)
    {
        string done = Localizer.GetText(LocalizationKeys.UiDone);
        string secLabel = Localizer.CurrentLanguage == Localizer.Language.English ? "sec." : "сек.";
        AnsiConsole.MarkupLine($"\n[green]{done}[/] ([grey]{elapsedSeconds:F1} {secLabel}[/])");
    }

    public void ShowResult(string text)
    {
        string header =
            "🧠 "
            + (
                Localizer.CurrentLanguage == Localizer.Language.English
                    ? "Recognized text"
                    : "Распознанный текст"
            );
        string empty =
            Localizer.CurrentLanguage == Localizer.Language.English
                ? "[italic grey]Empty result[/]"
                : "[italic grey]Пустой результат[/]";

        var panel = new Panel(string.IsNullOrWhiteSpace(text) ? empty : text)
            .Header(header)
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Green)
            .Padding(1, 1, 1, 1);

        AnsiConsole.Write(panel);
    }

    public void WaitForStart()
    {
        string msg =
            Localizer.CurrentLanguage == Localizer.Language.English
                ? "[yellow]▶ Press [bold]Enter[/] to start recording...[/]"
                : "[yellow]▶ Нажмите [bold]Enter[/] для начала записи...[/]";
        AnsiConsole.MarkupLine(msg);
        Console.ReadLine();
    }

    public void WaitForStop()
    {
        string msg =
            Localizer.CurrentLanguage == Localizer.Language.English
                ? "[yellow]⏹ Recording in progress. Press [bold]Enter[/] to stop...[/]"
                : "[yellow]⏹ Запись идёт. Нажмите [bold]Enter[/] для остановки...[/]";
        AnsiConsole.MarkupLine(msg);
        Console.ReadLine();
    }
}
