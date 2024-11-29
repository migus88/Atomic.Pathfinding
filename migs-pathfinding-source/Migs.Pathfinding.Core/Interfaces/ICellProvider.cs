using System;
using System.Runtime.CompilerServices;
using Migs.Pathfinding.Core.Data;

namespace Migs.Pathfinding.Core.Interfaces
{
    public unsafe interface ICellProvider
    {
        /// <summary>
        /// Width of the grid
        /// </summary>
        int Width { get; }
        /// <summary>
        /// Height of the grid
        /// </summary>
        int Height { get; }
        
        /// <summary>
        /// Returns the pointer to a cell at the specified position
        /// </summary>
        /// <param name="x">Horizontal position on the grid </param>
        /// <param name="y">Vertical position on the grid</param>
        /// <returns></returns>
        Cell* GetCellPointer(int x, int y);
        
        /// <summary>
        /// Resetting the cells to their default state <br/>
        /// Should call Cell.Reset() for each cell
        /// </summary>
        void ResetCells();
    }
}