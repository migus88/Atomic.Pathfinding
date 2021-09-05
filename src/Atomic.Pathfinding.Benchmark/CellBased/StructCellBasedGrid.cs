using System;
using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Interfaces;

namespace Atomic.Pathfinding.Benchmark.CellBased
{
    public struct StructCellBasedGrid : IGrid
    {
        public IGridCell[,] Matrix { get; }

        public StructCellBasedGrid(IGridCell[,] matrix)
        {
            Matrix = matrix;
        }

        public struct GridCell : IGridCell
        {
            public bool IsWalkable { get; set; }
            public bool IsOccupied { get; set; }
            public double Weight { get; set; }

            public GridCell(bool isWalkable, bool isOccupied, double weight)
            {
                IsWalkable = isWalkable;
                IsOccupied = isOccupied;
                Weight = weight;
            }
        }
    }
}