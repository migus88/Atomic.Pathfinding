using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Tools;

namespace Atomic.Pathfinding.Benchmarks.MazeBenchmarks;

public interface IMazeBenchmarkRunner
{
    void Init(Maze maze);
    void FindPathBenchmark((int x, int y) start, (int x, int y) destination);
    void RenderPath((int x, int y) start, (int x, int y) destination);
}