using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Helpers;
using Atomic.Pathfinding.Core.Internal;

namespace Atomic.Pathfinding.Core.Data
{
    public struct Cell
    {
        public bool IsClosed;
        public Coordinate Coordinate;
        public float F;
        public float H;
        public float G;
        public int Depth;
        public bool IsWalkable;
        public bool IsOccupied;
        public float Weight;
        public Coordinate ParentCoordinate;
        public int CellIndex;
        public int QueueIndex;

        public void Reset()
        {
            IsClosed = false;
            F = 0;
            H = 0;
            G = 0;
            Depth = 0;
            ParentCoordinate.Reset();
        }
    }
}