using Atomic.Pathfinding.Core;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Interfaces;
using BenchmarkDotNet.Attributes;

namespace Atomic.Pathfinding.Benchmark.CellBased
{
    [MemoryDiagnoser]
    public class CellBasedPathfinderBenchmarks
    {
        private const int GridWidth = 100;
        private const int GridHeight = 100;

        private IGridCell[,] _classMatrix = new IGridCell[GridWidth, GridHeight];
        private IGridCell[,] _structsMatrix = new IGridCell[GridWidth, GridHeight];

        private IAgent _agent = new Agent();

        public CellBasedPathfinderBenchmarks()
        {
            for (var i = 0; i < GridHeight; i++)
            {
                for (var j = 0; j < GridWidth; j++)
                {
                    _classMatrix[j, i] = new ClassCellBasedGrid.GridCell();
                    _structsMatrix[j, i] = new StructCellBasedGrid.GridCell(true, false, 0);
                }
            }
        }

        [Benchmark]
        public void ClassGridCreation()
        {
            var matrix = new IGridCell[GridWidth, GridHeight];
            for (var i = 0; i < GridHeight; i++)
            {
                for (var j = 0; j < GridWidth; j++)
                {
                    matrix[j, i] = new ClassCellBasedGrid.GridCell();
                }
            }
        }

        [Benchmark]
        public void StructGridCreation()
        {
            var matrix = new IGridCell[GridWidth, GridHeight];
            for (var i = 0; i < GridHeight; i++)
            {
                for (var j = 0; j < GridWidth; j++)
                {
                    matrix[j, i] = new StructCellBasedGrid.GridCell(true, false, 0);
                }
            }
        }

        [Benchmark]
        public void ClassGridPathfinding()
        {
            var grid = new ClassCellBasedGrid(_classMatrix);
            var pathfinder = new CellBasedPathfinder(grid);

            var start = new Coordinate {X = 0, Y = 0};
            var end = new Coordinate {X = GridWidth - 1, Y = GridHeight - 1};

            var result = pathfinder.GetPath(_agent, start, end);
        }

        [Benchmark]
        public void StructGridPathfinding()
        {
            var grid = new StructCellBasedGrid(_structsMatrix);
            var pathfinder = new CellBasedPathfinder(grid);

            var start = new Coordinate {X = 0, Y = 0};
            var end = new Coordinate {X = GridWidth - 1, Y = GridHeight - 1};

            var result = pathfinder.GetPath(_agent, start, end);
        }
    }
}