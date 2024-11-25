using Atomic.Pathfinding.Core.Data;

namespace Atomic.Pathfinding.Core.Interfaces
{
    public interface ICell
    {
        bool IsClosed { get; set; }
        Coordinate Coordinate { get; set; }
        float ScoreF { get; set; }
        float ScoreH { get; set; }
        float ScoreG { get; set; }
        int Depth { get; set; }
        bool IsWalkable { get; set; }
        bool IsOccupied { get; set; }
        float Weight { get; set; }
        Coordinate ParentCoordinate { get; set; }
        int QueueIndex { get; set; }
        void Reset();
    }
}