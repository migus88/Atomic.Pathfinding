using System;
using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Interfaces;

namespace Atomic.Pathfinding.Benchmark.CellBased
{
    public class ClassCellBasedGrid : IGrid
    {
        public IGridCell[,] Matrix { get; }

        public ClassCellBasedGrid(IGridCell[,] matrix)
        {
            Matrix = matrix;
        }

        public class GridCell : IGridCell
        {
            public bool IsWalkable { get; set; } = true;
            public bool IsOccupied { get; set; } = false;
            public double Weight { get; set; } = 0;
        }
    }
}