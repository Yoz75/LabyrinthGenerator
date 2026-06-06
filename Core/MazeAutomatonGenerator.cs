
using CTK.Engine;
using CTK.Engine.Rules;
using System.Collections.Generic;

namespace LabyrinthGenerator.Core;

/// <summary>
/// Labyrinth generator that generates the labyrinth using a cellular automaton powered by CLETKI
/// </summary>
internal class MazeAutomatonGenerator : ILabyrinthGenerator
{
    private readonly (int, int) Resolution;
    private readonly Cell PathType, WallType;
    private readonly CellTypeRegistrar Registrar;
    public MazeAutomatonGenerator((int, int) resolution)
    {
        Resolution = resolution;
        Registrar = new();

        PathType = Registrar.RegisterType();
        WallType = Registrar.RegisterType();
    }

    public CellType[,] Generate()
    {
        ICellEngine engine = CreateLabyrinthEngine();

        while(engine.CanUpdate())
        {
            engine.Update();
        }

        Field field = engine.GetState();
        CellType[,] resultField = GetField(field);

        return resultField;
    }

    private CellType[,] GetField(Field field)
    {
        (int, int) start = field.MyBounds.ValidStart;
        (int, int) end = field.MyBounds.ValidEnd;

        // TODO: when I'll release CLETKI 1.1.0 use CellTypeRegister.TypesCount
        CellType[] automaton2LabyrinthTypeMap = new CellType[Registrar.ExistingTypesCount];
        automaton2LabyrinthTypeMap[PathType.Type] = CellType.Path;
        automaton2LabyrinthTypeMap[WallType.Type] = CellType.Wall;

        var resultField = new CellType[end.Item1 - 1, end.Item2 - 1];
        for(int y = start.Item2; y < end.Item2; y++)
        {
            for(int x = start.Item1; x < end.Item1; x++)
            {
                resultField[x - start.Item1, y - start.Item2] = automaton2LabyrinthTypeMap[field.Map[x, y].Type];
            }
        }

        return resultField;
    }

    private ICellEngine CreateLabyrinthEngine()
    {
        AutomatonStage seedWallsStage = new(1, new RandomWrapperRule<AlwaysRule>(0.3f, new(WallType)));

        // Just something that depends on resolution
        AutomatonStage mazeStage = new(Resolution.Item1 * Resolution.Item2, 
            new StartTypeWrapperRule<NearRule>(PathType, new(WallType, WallType, 3)),
            new StartTypeWrapperRule<NearRule>(WallType, new(WallType, PathType, 0, 6, 7, 8)));

        AutomatonStage cleanupStage = new(1, new NearRule(Cell.Invalid, WallType, 1, 2, 3, 4, 5, 6, 7, 8));

        Queue<IAutomatonStage> stages = [];
        stages.Enqueue(seedWallsStage);
        stages.Enqueue(mazeStage);
        stages.Enqueue(cleanupStage);

        CTKEngine engine = new(Resolution, PathType, stages);

        return engine;
    }
}
