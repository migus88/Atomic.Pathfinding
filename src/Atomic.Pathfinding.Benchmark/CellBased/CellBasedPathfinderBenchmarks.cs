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

        private IGrid _classGrid;

        private TerrainPathfinder _classPathfinder;

        private IAgent _agent = new Agent();

        public CellBasedPathfinderBenchmarks()
        {
            for (var i = 0; i < GridHeight; i++)
            {
                for (var j = 0; j < GridWidth; j++)
                {
                    _classMatrix[j, i] = new ClassCellBasedGrid.GridCell();
                }
            }

            _classGrid = new ClassCellBasedGrid(_classMatrix);

            _classPathfinder = new TerrainPathfinder(_classGrid);
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
        public void ClassGridCreation()
        {
            var grid = new ClassCellBasedGrid(_classMatrix);
        }

        [Benchmark]
        public void ClassPathfinderCreation()
        {
            var pathfinder = new TerrainPathfinder(_classGrid);
        }

        [Benchmark]
        public void ClassGridPathfinding()
        {
            var start = new Coordinate(0, 0);
            var end = new Coordinate(GridWidth - 1, GridHeight - 1);

            var result = _classPathfinder.GetPath(_agent, start, end);
        }
    }
}