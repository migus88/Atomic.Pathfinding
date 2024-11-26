using System;
using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Data;

namespace Atomic.Pathfinding.Core.Interfaces
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