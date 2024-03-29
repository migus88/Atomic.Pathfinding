using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Atomic.Pathfinding.Core;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Helpers;
using Atomic.Pathfinding.Core.Interfaces;
using Atomic.Pathfinding.Core.Internal;
using BenchmarkDotNet.Attributes;

namespace Atomic.Pathfinding.Benchmark.Maze
{
    [MemoryDiagnoser]
    public class MazeBenchmark
    {
        private readonly Pathfinder<Cell> _pathfinder;
        private readonly IAgent _agent;

        private readonly Tools.Maze<Cell> _maze;
        private readonly Coordinate _start;
        private readonly Coordinate _destination;
        
        private readonly Tools.Maze<Cell> _testMaze;

        public MazeBenchmark()
        {
            _maze = new Tools.Maze<Cell>("cavern.gif");
            _testMaze = new Tools.Maze<Cell>("cavern.gif", false);
            
            _start = new Coordinate(10,10);
            _destination =  new Coordinate(502, 374);
            
            _maze.SetStart(_start);
            _maze.SetDestination(_destination);
            _agent = new Agent();
            
            _pathfinder = new Pathfinder<Cell>(_maze.Width, _maze.Height);
        }

        [Benchmark]
        public void CreatePathfinder()
        {
            var pathfinder = new Pathfinder<Cell>(_testMaze.Width, _testMaze.Height);
        }

        [Benchmark]
        public void Search()
        {
            var result = _pathfinder.GetPath(_maze, _agent, _start, _destination);

            if (!result.IsPathFound)
            {
                throw new Exception("Path not found");
            }
        }

        public void Render()
        {
            var result = _pathfinder.GetPath(_maze, _agent, _start, _destination);
            
            if(result.IsPathFound)
            {
                _maze.AddPath(result.Path);
            }

            if (!Directory.Exists("Results/"))
            {
                Directory.CreateDirectory("Results/");
            }
            
            _maze.SaveImage("Results/cavern.png", 4);
        }
    }
}