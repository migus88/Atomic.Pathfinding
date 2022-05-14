using System;
using System.IO;
using Atomic.Pathfinding.Core;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Interfaces;
using Atomic.Pathfinding.Tools;

namespace Atomic.Pathfinding.Benchmark.Maze
{
    public class AtomicMazeBenchmarkRunner : BaseMazeBenchmarkRunner
    {
        private Pathfinder<Cell> _pathfinder;
        private IAgent _agent;

        public override void Init(Maze<Cell> maze)
        {
            base.Init(maze);
            _agent = new Agent();
            _pathfinder = new Pathfinder<Cell>(_maze.Width, _maze.Height);
        }

        public override void FindPathBenchmark((int x, int y) start, (int x, int y) destination)
        {
            var result = _pathfinder.GetPath(_maze, _agent, (Coordinate)start, (Coordinate)destination);

            if (!result.IsPathFound)
            {
                throw new Exception("Path not found");
            }
        }

        public override void RenderPath((int x, int y) start, (int x, int y) destination)
        {
            var result = _pathfinder.GetPath(_maze, _agent, (Coordinate)start, (Coordinate)destination);
            
            if(result.IsPathFound)
            {
                _maze.AddPath(result.Path);
            }

            if (!Directory.Exists(ResultsPath))
            {
                Directory.CreateDirectory(ResultsPath);
            }
            
            _maze.SaveImage(ResultImagePath, 4);
        }
    }
}