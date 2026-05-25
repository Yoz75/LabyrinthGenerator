using LabyrinthGenerator.Core;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.ComponentModel;
using System.Threading;

namespace LabyrinthGenerator;

internal class GenerateCommand : Command<GenerateCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-w|--width <VALUE>")]
        [Description("The width of the labyrinth")]
        public int Width { get; init; } = 16;

        [CommandOption("-h|--height <VALUE>")]
        [Description("The height of the labyrinth")]
        public int Height { get; init; } = 16;

        [CommandOption("-c|--complexity <VALUE>")]
        [Description("The \"complexity\" of the labyrinth. The more this value, the more hard solution labyrinth has")]
        public int Complexity{ get; init; } = 16;

        [CommandOption("--type <\"extruder\"|[more coming soon...]>")]
        [Description("The type of the generator")]
        public AvailableGenerators GeneratorType { get; init; } = AvailableGenerators.Extruder;
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        const int minSize = 8;

        if(settings.Height < minSize || settings.Width < minSize)
        {
            AnsiConsole.WriteLine($"Both height and width must be >= {minSize}!");
            return 1;
        }

        ILabyrinthGenerator generator = settings.GeneratorType switch
        {
            AvailableGenerators.Extruder => new ExtruderGenerator(settings.Width, settings.Height, settings.Complexity),
            _ => throw new ArgumentException("Unknown generator type!")
        };

        ILabyrinthRenderer renderer = new AnsiConsoleRenderer();


        var labyrinth = generator.Generate();
        renderer.Render(labyrinth);
        return 0;
    }
}
