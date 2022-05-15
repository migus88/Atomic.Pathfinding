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
        protected override string ResultImageName => nameof(AtomicMazeBenchmarkRunner);
        
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
            
            if(!result.IsPathFound)
            {
                return;
            }
            
            foreach (var cell in _maze.Cells)
            {
                if (cell.IsClosed)
                {
                    _maze.SetClosed(cell.Coordinate);
                }
            }
            
            _maze.AddPath(result.Path);
            
            SaveMazeResultAsImage();
        }
    }
}