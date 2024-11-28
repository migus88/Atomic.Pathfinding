using Atomic.Pathfinding.Core;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Interfaces;
using Atomic.Pathfinding.Tools;
using BenchmarkDotNet.Attributes;

namespace Atomic.Pathfinding.Benchmarks;

[MemoryDiagnoser]
public class InternalBenchmarkRunner
{
    private readonly Maze _maze = new("cavern.gif");
    private readonly IAgent _agent = new Agent();
    private Pathfinder _pathfinder;

    public InternalBenchmarkRunner()
    {
        _pathfinder = new Pathfinder(_maze);
    }

    [Benchmark]
    public void Initialization()
    {
        var pathfinder = new Pathfinder(_maze);
    }

    [Benchmark]
    public void PathFinding() => FindPath((10, 10), (502, 374));

    [Benchmark]
    public unsafe void CopyingResultToArray()
    {
        var result = FindPath((10, 10), (502, 374));
        var array = stackalloc Coordinate[result.Path.Length];

        for (int i = 0; i < result.Path.Length; i++)
        {
            array[i] = result.Path[i];
        }
    }
    
    public PathResult FindPath((int x, int y) start, (int x, int y) destination)
    {
        if (_pathfinder == null)
        {
            throw new Exception("Pathfounder is not initialized");
        }
        
        var result = _pathfinder.GetPath(_agent, (Coordinate)start, (Coordinate)destination);

        if (!result.IsPathFound)
        {
            throw new Exception("Path not found");
        }

        return result;
    }
}