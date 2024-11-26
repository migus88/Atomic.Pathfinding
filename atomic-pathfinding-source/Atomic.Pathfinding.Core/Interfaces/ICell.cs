using Atomic.Pathfinding.Core.Data;

namespace Atomic.Pathfinding.Core.Interfaces
{
    public interface ICell
    {
        Coordinate Coordinate { get; set; }
        bool IsWalkable { get; set; }
        bool IsOccupied { get; set; }
        float Weight { get; set; }
        
        void Reset();
    }
}