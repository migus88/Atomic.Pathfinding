using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Atomic.Pathfinding.Benchmark.CellBased;
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
        public int FailCount { get; private set; } = 0;
        public int SuccessCount { get; private set; } = 0;
        public int TotalCount { get; private set; } = 0;
        
        private readonly TerrainPathfinder _pathfinder;
        private readonly IAgent _agent;
        private readonly Cell[] _cells;

        private readonly Tools.Maze _maze;
        private readonly Coordinate _start;
        private readonly Coordinate _destination;

        public MazeBenchmark()
        {
            _maze = new Tools.Maze("cavern.gif");
            
            _start = new Coordinate(10,10);
            _destination =  new Coordinate(502, 374);
            
            _maze.SetStart(_start);
            _maze.SetDestination(_destination);
            _cells = _maze.Cells;
            _agent = new Agent();
            
            _pathfinder = new TerrainPathfinder(_maze.Width, _maze.Height);
        }

        [Benchmark]
        public void Find()
        {
            TotalCount++;
            var result = _pathfinder.GetPath(_cells, _agent, _start, _destination);

            if (!result.IsPathFound)
            {
                throw new Exception("FUCK!");
            }
        }

        public void Render()
        {
            var result = _pathfinder.GetPath(_cells, _agent, _start, _destination);
            
            if(result.IsPathFound)
            {
                _maze.AddPath((Coordinate[]) result.Path);
            }

            if (!Directory.Exists("Results/"))
            {
                Directory.CreateDirectory("Results/");
            }
            
            _maze.SaveImage("Results/cavern.png", 4);
        }
    }
}