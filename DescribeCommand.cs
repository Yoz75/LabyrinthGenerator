using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.ComponentModel;
using System.Threading;

namespace LabyrinthGenerator;

internal sealed class DescribeCommand : Command<DescribeCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<generator name>")]
        [Description("The name of generator to be described")]
        public AvailableGenerators Generator { get; init; }
    }
    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        switch(settings.Generator)
        {
            case AvailableGenerators.Extruder:
                AnsiConsole.WriteLine("Extruder generator is a generator that makes a random main path " +
                    "and extrudes dead ends from it. Uses complexity to define the count of dead ends.");
                break;
            case AvailableGenerators.MazeCE:
                AnsiConsole.WriteLine("MazeCE is the Maze cellular automaton. It doesn't generate " +
                    "a real valid labyrinth (with both enter and exit). It is only a labyrinth parody.");
                break;
            default:
                throw new InvalidEnumArgumentException($"Unknown generator type {settings.Generator}!");
        }

        return 0;
    }

}
