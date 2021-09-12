using Atomic.Pathfinding.Core.Data;

namespace Atomic.Pathfinding.Core.Interfaces
{
    public unsafe interface ICellProvider<T> where T : unmanaged, ICell
    {
        T* GetCellPointer(float x, float y);
        T* GetCellPointer(Coordinate coordinate);
        void ResetCells();
    }
}