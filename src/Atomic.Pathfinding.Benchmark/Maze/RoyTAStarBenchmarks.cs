using System;
using Atomic.Pathfinding.Core.Data;
using BenchmarkDotNet.Attributes;
using Roy_T.AStar.Graphs;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;

namespace Atomic.Pathfinding.Benchmark.Maze
{
    //Implementation URL:
    //https://github.com/roy-t/AStar
    
    [MemoryDiagnoser]
    public class RoyTAStarBenchmarks
    {
        private readonly Tools.Maze<Cell> _maze;
        private readonly PathFinder _pathFinder;
        private readonly Node[,] _nodes;
        private readonly GridPosition _start;
        private readonly GridPosition _destination;
        private readonly Grid _grid;
        
        public RoyTAStarBenchmarks()
        {
            _maze = new Tools.Maze<Cell>("cavern.gif");

            _start = new GridPosition(10, 10);
            _destination = new GridPosition(502, 374);
            
            var gridSize = new GridSize(_maze.Width, _maze.Height);
            var cellSize = new Size(Distance.FromMeters(1), Distance.FromMeters(1));
            _pathFinder = new PathFinder();

            _nodes = new Node[_maze.Width, _maze.Height];
            PopulateNodes();

            _grid = Grid.CreateGridFrom2DArrayOfNodes(_nodes);
        }

        [Benchmark]
        public void Search()
        {
            var path = _pathFinder.FindPath(_start, _destination, _grid);

            if (path.Edges.Count == 0)
            {
                throw new Exception("Path not found");
            }
        }

        private void PopulateNodes()
        {
            var cells = _maze.Cells;
            
            for (int y = 0; y < _maze.Height; y++)
            {
                for (int x = 0; x < _maze.Width; x++)
                {
                    _nodes[x, y] = new Node(new Position(x, y));
                }
            }
            
            for (int y = 0; y < _maze.Height; y++)
            {
                for (int x = 0; x < _maze.Width; x++)
                {
                    Connect(x, y, cells);
                }
            }
        }

        private void Connect(int x, int y, Cell[,] cells)
        {
            var node = _nodes[x, y];

            var neighbors = new (int, int)[]
            {
                (x + 1, y),
                (x - 1, y),
                (x, y + 1),
                (x, y - 1),
                (x + 1, y + 1),
                (x - 1, y - 1),
                (x + 1, y - 1),
                (x - 1, y + 1)
            };

            var velocity = Velocity.FromMetersPerSecond(1);

            foreach (var neighbor in neighbors)
            {
                if (neighbor.Item1 < 0 || neighbor.Item1 >= _maze.Width || neighbor.Item2 < 0 ||
                    neighbor.Item2 >= _maze.Height)
                {
                    continue;
                }

                var cell = cells[neighbor.Item1, neighbor.Item2];
                if (cell.IsOccupied || !cell.IsWalkable)
                {
                    continue;
                }
                
                node.Connect(_nodes[neighbor.Item1, neighbor.Item2], velocity);
            }
        }
    }
}