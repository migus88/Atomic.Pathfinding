using System.Reflection;
using Migs.Pathfinding.Core;
using Migs.Pathfinding.Core.Data;
using Migs.Pathfinding.Core.Interfaces;
using Migs.Pathfinding.Tools;

namespace Migs.Pathfinding.Benchmarks.MazeBenchmarks;

public class MigsMazeBenchmarkRunner : BaseMazeBenchmarkRunner
{
    protected override string ResultImageName => nameof(MigsMazeBenchmarkRunner);
        
    private Pathfinder _pathfinder;
    private IAgent _agent;

    public override void Init(Maze maze)
    {
        base.Init(maze);
        _agent = new Agent();

        _pathfinder = new Pathfinder(_maze);
    }

    public override void FindPath((int x, int y) start, (int x, int y) destination)
    {
        if (_pathfinder == null)
        {
            return;
        }
        var result = _pathfinder.GetPath(_agent, (Coordinate)start, (Coordinate)destination);

        if (!result.IsPathFound)
        {
            throw new Exception("Path not found");
        }
    }

    public override void RenderPath((int x, int y) start, (int x, int y) destination)
    {
        var result = _pathfinder.GetPath(_agent, (Coordinate)start, (Coordinate)destination);
            
        if(!result.IsPathFound)
        {
            return;
        }
        
        
        var propertyInfo = typeof(Cell).GetProperty("IsClosed", BindingFlags.NonPublic | BindingFlags.Instance);
        
        foreach (var cell in _maze.Cells)
        {
            if ((bool)(propertyInfo?.GetValue(cell) ?? false))
            {
                _maze.SetClosed(cell.Coordinate);
            }
        }
            
        _maze.AddPath(result.Path.ToArray());
            
        SaveMazeResultAsImage();
    }
}