namespace LabyrinthGenerator.Core;

internal enum CellType : byte
{ 
    None = 0,
    Wall,
    Path
}

/// <summary>
/// Something that generates a labyrinth
/// </summary>
internal interface ILabyrinthGenerator
{
    public CellType[,] Generate();
}
