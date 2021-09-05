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

        private IGrid _classGrid;
        private IGrid _structGrid;

        private CellBasedPathfinder _classPathfinder;
        private CellBasedPathfinder _structPathfinder;

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

            _classGrid = new ClassCellBasedGrid(_classMatrix);
            _structGrid = new StructCellBasedGrid(_structsMatrix);

            _classPathfinder = new CellBasedPathfinder(_classGrid);
            _structPathfinder = new CellBasedPathfinder(_structGrid);
        }

        [Benchmark]
        public void ClassMatrixCreation()
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
        public void StructMatrixCreation()
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
        public void ClassGridCreation()
        {
            var grid = new ClassCellBasedGrid(_classMatrix);
        }

        [Benchmark]
        public void StructGridCreation()
        {
            var grid = new ClassCellBasedGrid(_structsMatrix);
        }

        [Benchmark]
        public void ClassPathfinderCreation()
        {
            var pathfinder = new CellBasedPathfinder(_classGrid);
        }

        [Benchmark]
        public void StructPathfinderCreation()
        {
            var pathfinder = new CellBasedPathfinder(_structGrid);
        }

        [Benchmark]
        public void ClassGridPathfinding()
        {
            var start = new Coordinate {X = 0, Y = 0};
            var end = new Coordinate {X = GridWidth - 1, Y = GridHeight - 1};

            var result = _classPathfinder.GetPath(_agent, start, end);
        }

        [Benchmark]
        public void StructGridPathfinding()
        {
            var start = new Coordinate {X = 0, Y = 0};
            var end = new Coordinate {X = GridWidth - 1, Y = GridHeight - 1};

            var result = _structPathfinder.GetPath(_agent, start, end);
        }
    }
}