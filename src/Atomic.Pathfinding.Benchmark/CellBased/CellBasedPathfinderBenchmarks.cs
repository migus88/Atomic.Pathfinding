using Atomic.Pathfinding.Core;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Interfaces;
using Atomic.Pathfinding.Core.Internal;
using BenchmarkDotNet.Attributes;

namespace Atomic.Pathfinding.Benchmark.CellBased
{
    [MemoryDiagnoser]
    public class CellBasedPathfinderBenchmarks
    {
        private const int GridWidth = 100;
        private const int GridHeight = 100;

        private TerrainPathfinder _classPathfinder;

        private IAgent _agent = new Agent();
        private Cell[] _cells;

        public CellBasedPathfinderBenchmarks()
        {
            _classPathfinder = new TerrainPathfinder(GridWidth, GridHeight);

            _cells = new Cell[GridWidth * GridHeight];

            var index = 0;
            for (short x = 0; x < GridWidth; x++)
            {
                for (short y = 0; y < GridHeight; y++)
                {
                    ref var cell = ref _cells[index];
                    cell.Coordinate = new Coordinate(x, y);
                    index++;
                }
            }
        }

        [Benchmark]
        public void ClassMatrixCreation()
        {
            var index = 0;
            var matrix = new IGridCell[GridWidth, GridHeight];
            for (short y = 0; y < GridHeight; y++)
            {
                for (short x = 0; x < GridWidth; x++)
                {
                    matrix[x, y] = new ClassCellBasedGrid.GridCell();
                    ref var cell = ref _cells[index];
                    cell.Coordinate = new Coordinate(x, y);
                    index++;
                }
            }

            var cells = new Cell[GridWidth * GridHeight];

            for (short x = 0; x < GridWidth; x++)
            {
                for (short y = 0; y < GridHeight; y++)
                {
                }
            }
        }

        [Benchmark]
        public void ClassPathfinderCreation()
        {
            var pathfinder = new TerrainPathfinder(GridWidth, GridHeight);
        }

        [Benchmark]
        public void ClassGridPathfinding()
        {
            var start = new Coordinate(0, 0);
            var end = new Coordinate(GridWidth - 1, GridHeight - 1);

            var result = _classPathfinder.GetPath(_cells, _agent, start, end);
        }
    }
}