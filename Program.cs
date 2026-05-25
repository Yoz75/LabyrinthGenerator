using Spectre.Console;
using Spectre.Console.Cli;

namespace LabyrinthGenerator;

internal class Program
{
    static void Main(string[] args)
    {
        var app = new CommandApp<GenerateCommand>();
        app.Configure(config =>
        {
            config.SetExceptionHandler((ex, resolver) =>
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            });
        });
        app.Run(args);
    }
}
