using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Helpers;
using Atomic.Pathfinding.Core.Internal;

namespace Atomic.Pathfinding.Core.Data
{
    public struct Cell
    {
        public bool IsClosed { get; private set; }
        public Coordinate Coordinate { get; private set; }
        public float F { get; private set; }
        public float H { get; private set; }
        public float G { get; private set; }
        public int Depth { get; private set; }
        
        public bool IsWalkable { get; private set; }
        
        public bool IsOccupied { get; private set; }
        
        public float Weight { get; private set; }
        public Coordinate ParentCoordinate { get; private set; }
        public bool IsInitialized { get; private set; }
        public PriorityQueueItem QueueItem { get; private set; }

        public void Reset(int cellIndex)
        {
            IsClosed = false;
            F = 0;
            H = 0;
            G = 0;
            Depth = 0;
            ParentCoordinate.Reset();
            IsInitialized = true;
        }

        public void SetIsWalkable(bool isWalkable)
        {
            IsWalkable = isWalkable;
        }

        public void SetIsOccupied(bool isOccupied)
        {
            IsOccupied = isOccupied;
        }

        public void SetWeight(float weight)
        {
            Weight = weight;
        }

        public void SetQueueItem(PriorityQueueItem queueItem)
        {
            QueueItem = queueItem;
        }
        
        //TODO: Automate this initialization
        public void InitQueueItem(int cellIndex)
        {
            var queueItem = new PriorityQueueItem(cellIndex);
            SetQueueItem(queueItem);
        }

        public void SetIsClosed(bool isClosed)
        {
            IsInitialized = true;
            IsClosed = isClosed;
        }

        public void SetCoordinate(Coordinate coordinate)
        {
            IsInitialized = true;
            Coordinate = coordinate;
        }

        public void SetF(float f)
        {
            IsInitialized = true;
            F = f;
        }

        public void SetH(float h)
        {
            IsInitialized = true;
            H = h;
        }

        public void SetG(float g)
        {
            IsInitialized = true;
            G = g;
        }

        public void SetDepth(int depth)
        {
            IsInitialized = true;
            Depth = depth;
        }

        public void SetParentCoordinate(Coordinate coordinate)
        {
            IsInitialized = true;
            ParentCoordinate = coordinate;
        }
    }
}