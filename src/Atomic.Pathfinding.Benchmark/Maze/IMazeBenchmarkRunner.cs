using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Tools;

namespace Atomic.Pathfinding.Benchmark.Maze
{
    public interface IMazeBenchmarkRunner
    {
        void Init(Maze<Cell> maze);
        void FindPathBenchmark((int x, int y) start, (int x, int y) destination);
        void RenderPath((int x, int y) start, (int x, int y) destination);
    }
}