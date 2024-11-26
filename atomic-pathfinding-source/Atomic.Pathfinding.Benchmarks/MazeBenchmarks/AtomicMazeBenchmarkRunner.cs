using System.Reflection;
using Atomic.Pathfinding.Core;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Interfaces;
using Atomic.Pathfinding.Tools;

namespace Atomic.Pathfinding.Benchmarks.MazeBenchmarks;

public class AtomicMazeBenchmarkRunner : BaseMazeBenchmarkRunner
{
    protected override string ResultImageName => nameof(AtomicMazeBenchmarkRunner);
        
    private Pathfinder _pathfinder;
    private IAgent _agent;
    private PathResult _result;

    public override void Init(Maze maze)
    {
        base.Init(maze);
        _agent = new Agent();
        _pathfinder = new Pathfinder(_maze.Width, _maze.Height);
    }

    public override void FindPathBenchmark((int x, int y) start, (int x, int y) destination)
    {
        _result = _pathfinder.GetPath(_maze, _agent, (Coordinate)start, (Coordinate)destination);

        if (!_result.IsPathFound)
        {
            throw new Exception("Path not found");
        }
    }

    public override void RenderPath((int x, int y) start, (int x, int y) destination)
    {
        var result = _pathfinder.GetPath(_maze, _agent, (Coordinate)start, (Coordinate)destination);
            
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
            
        _maze.AddPath(result.Path);
            
        SaveMazeResultAsImage();
    }
}