using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Helpers;
using Atomic.Pathfinding.Core.Interfaces;
using Atomic.Pathfinding.Core.Internal;

namespace Atomic.Pathfinding.Core.Data
{
    public struct Cell : ICell
    {
        public bool IsClosed { get; set; }
        public Coordinate Coordinate { get; set; }
        public float ScoreF { get; set; }
        public float ScoreH { get; set; }
        public float ScoreG { get; set; }
        public int Depth { get; set; }
        public bool IsWalkable { get; set; }
        public bool IsOccupied { get; set; }
        public float Weight { get; set; }
        public Coordinate ParentCoordinate { get; set; }
        public int QueueIndex { get; set; }
        
        public void Reset()
        {
            IsClosed = false;
            ScoreF = 0;
            ScoreH = 0;
            ScoreG = 0;
            Depth = 0;
            ParentCoordinate.Reset();
        }
    }
}