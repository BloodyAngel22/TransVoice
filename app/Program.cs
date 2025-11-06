using AudioListener.CLI;
using Spectre.Console.Cli;

var app = new CommandApp();

app.SetDefaultCommand<RecordAndDecodeCommand>();

return app.Run(args);
