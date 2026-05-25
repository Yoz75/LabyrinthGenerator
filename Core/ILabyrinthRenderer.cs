
namespace LabyrinthGenerator.Core;

/// <summary>
/// Something that renders a labyrinth
/// </summary>
internal interface ILabyrinthRenderer
{
    public void Render(CellType[,] labyrinth);
}
