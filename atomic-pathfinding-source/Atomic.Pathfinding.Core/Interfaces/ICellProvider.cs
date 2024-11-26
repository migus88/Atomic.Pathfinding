using System;
using System.Runtime.CompilerServices;

namespace Atomic.Pathfinding.Core.Interfaces
{
    public interface ICellProvider
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IntPtr GetCellPointer(int x, int y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ResetCells();
    }
}