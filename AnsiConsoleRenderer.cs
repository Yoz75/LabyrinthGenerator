using LabyrinthGenerator.Core;
using Spectre.Console;
using System.Collections.Generic;

namespace LabyrinthGenerator;

internal class AnsiConsoleRenderer : ILabyrinthRenderer
{
    private struct Color
    {
        public byte R, G, B;

        public Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public override string ToString()
        {
            const string hexFormat = "X2";
            return $"#{R.ToString(hexFormat)}" +
                    $"{G.ToString(hexFormat)}" +
                    $"{B.ToString(hexFormat)}";
        }
    }

    private struct RenderCell
    {
        public string Symbol;
        public Color Color;

        public RenderCell(string symbol, Color color)
        {
            Symbol = symbol;
            Color = color;
        }
    }

    private readonly Dictionary<CellType, RenderCell> Type2Cell = new()
    {
        {CellType.None, new("??", new(255, 0, 255))},
        {CellType.Wall, new("##", new(128, 128, 255))},
        {CellType.Path, new("..", new(128, 128, 192))}
    };

    public void Render(CellType[,] labyrinth)
    {
        for(int y = 0; y < labyrinth.GetLength(1); y++)
        {
            for(int x = 0; x < labyrinth.GetLength(0); x++)
            {
                var cell = Type2Cell[labyrinth[x, y]];
                AnsiConsole.Markup($"[{cell.Color}]{cell.Symbol}[/]");
            }
            AnsiConsole.WriteLine();
        }
    }
}
