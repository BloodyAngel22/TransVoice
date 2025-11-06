using AudioListener.Core;
using Spectre.Console;

namespace AudioListener.CLI;

public class ConsoleUI
{
    public void ShowHeader()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("TransVoice").Color(Color.Cyan1).Centered());
    }

    public void Section(string title)
    {
        AnsiConsole.Write(new Rule($"[yellow]{title}[/]").LeftJustified());
    }

    public void Info(string message)
    {
        AnsiConsole.MarkupLine($"[blue]ℹ[/] {message}");
    }

    public void Success(string message)
    {
        AnsiConsole.MarkupLine($"[green]✅[/] {message}");
    }

    public void Error(string message)
    {
        AnsiConsole.MarkupLine($"[red]❌[/] {message}");
    }

    public void Warning(string message)
    {
        AnsiConsole.MarkupLine($"[yellow]⚠[/] {message}");
    }

    public void Detail(string message)
    {
        AnsiConsole.MarkupLine($"[grey]•[/] {message}");
    }

    public async Task<string> RunWithSpinner(string title, Func<Task<string>> action)
    {
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
        table.AddColumn("[cyan]Свойство[/]");
        table.AddColumn("[green]Значение[/]");
        table.AddRow("Файл", info.FileName);
        table.AddRow("Путь", info.FilePath);
        table.AddRow("Длительность", $"{info.Duration:F2} сек.");

        AnsiConsole.Write(table);
    }

    public void ShowElapsedTime(double elapsedSeconds)
    {
        AnsiConsole.MarkupLine(
            $"\n[green]✅ Распознавание завершено![/] ([grey]{elapsedSeconds:F1} сек.[/])"
        );
    }

    public void ShowResult(string text)
    {
        var panel = new Panel(
            string.IsNullOrWhiteSpace(text) ? "[italic grey]Пустой результат[/]" : text
        )
            .Header("🧠 Распознанный текст")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Green)
            .Padding(1, 1, 1, 1);

        AnsiConsole.Write(panel);
    }

    public void WaitForStart()
    {
        AnsiConsole.MarkupLine("[yellow]▶ Нажмите [bold]Enter[/] для начала записи...[/]");
        Console.ReadLine();
    }

    public void WaitForStop()
    {
        AnsiConsole.MarkupLine("[yellow]⏹ Запись идёт. Нажмите [bold]Enter[/] для остановки...[/]");
        Console.ReadLine();
    }
}
