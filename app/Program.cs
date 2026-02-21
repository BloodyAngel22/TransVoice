using System.Globalization;
using app.CLI;
using app.Core;
using Spectre.Console.Cli;

var settings = UserSettings.LoadOrCreate();
var culture = settings.InterfaceLanguage.ToLower() switch
{
    "en" => new CultureInfo("en-US"),
    _ => new CultureInfo("ru-RU"),
};

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var app = new CommandApp<RecordAndDecodeCommand>();

app.Configure(configurator =>
{
    configurator.AddCommand<SettingsCommand>("settings");
});

return app.Run(args);
