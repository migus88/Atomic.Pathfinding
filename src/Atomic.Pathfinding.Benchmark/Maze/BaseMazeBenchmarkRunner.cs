using System.IO;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Tools;

namespace Atomic.Pathfinding.Benchmark.Maze
{
    public abstract class BaseMazeBenchmarkRunner : IMazeBenchmarkRunner
    {
        protected const string ResultsPath = "Results/";
        
        protected abstract string ResultImageName { get; }
        
        protected Maze<Cell> _maze;

        public virtual void Init(Maze<Cell> maze)
        {
            _maze = maze ?? new Maze<Cell>("cavern.gif");
        }

        public abstract void FindPathBenchmark((int x, int y) start, (int x, int y) destination);

        public abstract void RenderPath((int x, int y) start, (int x, int y) destination);

        protected void SaveMazeResultAsImage()
        {
            if (!Directory.Exists(ResultsPath))
            {
                Directory.CreateDirectory(ResultsPath);
            }

            var imagePath = $"{ResultsPath}{ResultImageName}.png";
            
            _maze.SaveImage(imagePath, 4);
        }
    }
}