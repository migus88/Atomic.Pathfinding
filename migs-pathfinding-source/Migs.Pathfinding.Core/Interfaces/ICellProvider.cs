using System;
using System.Runtime.CompilerServices;
using Migs.Pathfinding.Core.Data;

namespace Migs.Pathfinding.Core.Interfaces
{
    public unsafe interface ICellProvider
    {
        int Width { get; }
        int Height { get; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Cell* GetCellPointer(int x, int y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ResetCells();
    }
}