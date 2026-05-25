using System;
using System.Collections.Generic;

namespace LabyrinthGenerator.Core;

/// <summary>
/// Generates main path and extrudes it to make a dead end
/// </summary>
internal class ExtruderGenerator : ILabyrinthGenerator
{
    private struct Dummy;

    private struct Position
    {
        public int X, Y;

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Position left, Position right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }
    }

    private struct Neighbors
    {
        public Position Upper, Right, Down, Left;
    }

    private enum ExtruderCellType : byte
    {
        None = 0,
        Wall,
        Path,
        /// <summary>
        /// A wall that can not be broken (e.i dead ends can't extrude here etc)
        /// </summary>
        UnbreakableWall
    }

    private readonly int Width, Height, DeadEndSamples;
    private readonly int MinimalStepsCount;
    private int Steps = 0;
    private ExtruderCellType[,] Map;

    public ExtruderGenerator(int width, int height, int deadEndsSamples)
    {
        Width = width;
        Height = height;
        DeadEndSamples = deadEndsSamples;
        // I`ve just needed something that depends on the size of the labyrinth
        MinimalStepsCount = width + height;
    }

    public CellType[,] Generate()
    {
        Map = new ExtruderCellType[Width, Height];

        AddOuterBorders();
        GenerateMainPath();



        for(int i = 0; i < DeadEndSamples; i++)
        {
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    if(Map[x, y] == ExtruderCellType.UnbreakableWall)
                    {
                        Map[x, y] = ExtruderCellType.None;
                    }
                }
            }
            GenerateSidePath();
        }

        for(int y = 0; y < Height; y++)
        {
            for(int x = 0; x < Width; x++)
            {
                if(Map[x, y] == ExtruderCellType.None || Map[x, y] == ExtruderCellType.UnbreakableWall)
                {
                    Map[x, y] = ExtruderCellType.Wall;
                }
            }
        }

        return (CellType[,])(object)Map;
    }

    private void AddOuterBorders()
    {
        for(int x = 0; x < Width; x++)
        {
            Map[x, 0] = ExtruderCellType.Wall;
            Map[x, Height - 1] = ExtruderCellType.Wall;
        }

        for(int y = 0; y < Height; y++)
        {
            Map[0, y] = ExtruderCellType.Wall;
            Map[Width - 1, y] = ExtruderCellType.Wall;
        }
    }

    private void GenerateMainPath()
    {
        Position[] availablePositions =
        [
            // Vertical
            new Position(1, Random.Shared.Next(1, Height - 1)),
            new Position(Width - 2, Random.Shared.Next(1, Height - 1)),

            // Horizontal
            new Position(Random.Shared.Next(1, Width - 1), 1),
            new Position(Random.Shared.Next(1, Width - 1), Height - 2),
        ];

        var startPosition = availablePositions[Random.Shared.Next(availablePositions.Length)];

        var lastPosition = GeneratePath(startPosition);
        var borderPosition = TryGetBorderNeighbor(lastPosition) ?? throw new InvalidOperationException("Could not generate main path!");

        SetTypeOf(TryGetBorderNeighbor(startPosition)!.Value, ExtruderCellType.Path);
        SetTypeOf(borderPosition, ExtruderCellType.Path);
    }

    private void GenerateSidePath()
    {
        List<Position> availablePositions = [];

        for(int y = 1; y < Height - 1; y++)
        {
            for(int x = 1; x < Width - 1; x++)
            {
                var position = new Position(x, y);
                if(GetTypeOf(position) == ExtruderCellType.Path) continue;

                if(GetNeighborsCountOfType(position, ExtruderCellType.Path) != 1) continue;
                availablePositions.Add(new Position(x, y));
            }
        }

        if(availablePositions.Count <= 0) return;
        var selectedPosition = availablePositions[Random.Shared.Next(availablePositions.Count)];

        // GeneratePath throws an InvalidOperationException when can't reach wall
        // But we don`t care we just need a dead end
        try { GeneratePath(selectedPosition); }
        catch(InvalidOperationException) { }
    }

    private Position GeneratePath(Position start)
    {
        Stack<Position> path = new();

        SetTypeOf(start, ExtruderCellType.Path);
        path.Push(start);

        while(path.Count > 0)
        {
            Position current = path.Peek();

            Position? next = TryPickRandomFreeNeighbor(current);

            if(next.HasValue && IsBorder(next.Value))
            {
                return current;
            }

            if(next.HasValue)
            {
                SetTypeOf(next.Value, ExtruderCellType.Path);

                path.Push(next.Value);
            }
            else
            {
                Position deadEnd = path.Pop();

                if(path.Count > 0)
                {
                    SetTypeOf(deadEnd, ExtruderCellType.UnbreakableWall);
                }
            }

            Steps++;
        }

        throw new InvalidOperationException(
            "Could not generate path!");
    }

    private int GetNeighborsCountOfType(Position position, ExtruderCellType type)
    {
        int count = 0;
        Neighbors neighbors = GetNeighbors(position);

        if(GetTypeOf(neighbors.Upper) == type) count++;
        if(GetTypeOf(neighbors.Right) == type) count++;
        if(GetTypeOf(neighbors.Down) == type) count++;
        if(GetTypeOf(neighbors.Left) == type) count++;

        return count;
    }

    private bool IsBorder(Position position)
    {
        return position.X == 0 || position.Y == 0 || position.X == Width - 1 || position.Y == Height - 1;
    }

    private Position? TryPickRandomFreeNeighbor(Position position)
    {
        bool IsValidPathCandidate(Position candidate, Position current)
        {
            var type = GetTypeOf(candidate);
            if(type != ExtruderCellType.None && type != ExtruderCellType.Wall)
                return false;

            if(type == ExtruderCellType.Wall && Steps < MinimalStepsCount) return false;

            Neighbors neighbors_ = GetNeighbors(candidate);

            int count = 0;

            bool connectedToCurrent = false;

            void Check(Position p)
            {
                if(GetTypeOf(p) == ExtruderCellType.Path)
                {
                    count++;

                    if(p == current)
                        connectedToCurrent = true;
                }
            }

            if(type == ExtruderCellType.Wall) return true;
            Check(neighbors_.Upper);
            Check(neighbors_.Right);
            Check(neighbors_.Down);
            Check(neighbors_.Left);

            return count == 0 ||
                   (count == 1 && connectedToCurrent);
        }

        Neighbors neighbors = GetNeighbors(position);

        Position[] candidates =
        [
            neighbors.Upper,
            neighbors.Right,
            neighbors.Down,
            neighbors.Left
        ];

        // Fisher-Yates shuffle
        for(int i = candidates.Length - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);
            (candidates[i], candidates[j]) =
                (candidates[j], candidates[i]);
        }

        foreach(var candidate in candidates)
        {
            if(IsValidPathCandidate(candidate, position))
                return candidate;
        }

        return null;
    }
    private Neighbors GetNeighbors(Position position)
    {
        Neighbors neighbors = default;
        neighbors.Upper = new Position(position.X, position.Y - 1);
        neighbors.Down = new Position(position.X, position.Y + 1);
        neighbors.Right = new Position(position.X + 1, position.Y);
        neighbors.Left = new Position(position.X - 1, position.Y);

        return neighbors;
    }

    private Position? TryGetBorderNeighbor(Position position)
    {
        if(position.X == 1) return new Position(0, position.Y);
        else if(position.X == Width - 2) return new Position(Width - 1, position.Y);

        if(position.Y == 1) return new Position(position.X, 0);
        else if(position.Y == Height - 2) return new Position(position.X, Height - 1);

        return null;
    }

    private Position? TryGetNeighborOfType(Position position, ExtruderCellType type)
    {
        Neighbors neighbors = GetNeighbors(position);
        if(GetTypeOf(neighbors.Upper) == type) return neighbors.Upper;
        else if(GetTypeOf(neighbors.Right) == type) return neighbors.Right;
        else if(GetTypeOf(neighbors.Down) == type) return neighbors.Down;
        else if(GetTypeOf(neighbors.Left) == type) return neighbors.Left;

        return null;
    }

    private ExtruderCellType GetTypeOf(Position position)
    {
        return Map[position.X, position.Y];
    }

    private void SetTypeOf(Position position, ExtruderCellType type)
    {
        Map[position.X, position.Y] = type;
    }
}
