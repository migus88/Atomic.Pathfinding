using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Data;

namespace Atomic.Pathfinding.Core.Interfaces
{
    public unsafe interface ICellProvider<T> where T : unmanaged, ICell
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T* GetCellPointer(int x, int y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ResetCells();
    }
}