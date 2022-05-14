using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Tools;

namespace Atomic.Pathfinding.Benchmark.Maze
{
    public abstract class BaseMazeBenchmarkRunner : IMazeBenchmarkRunner
    {
        protected const string ResultsPath = "Results/";
        protected const string ResultImagePath = ResultsPath + "cavern.png";
        
        protected Maze<Cell> _maze;

        public virtual void Init(Maze<Cell> maze)
        {
            _maze = maze ?? new Maze<Cell>("cavern.gif");
        }

        public abstract void FindPathBenchmark((int x, int y) start, (int x, int y) destination);

        public abstract void RenderPath((int x, int y) start, (int x, int y) destination);
    }
}