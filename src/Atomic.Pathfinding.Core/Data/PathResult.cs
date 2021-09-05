using System.Collections.Generic;

namespace Atomic.Pathfinding.Core.Data
{
    public struct PathResult
    {
        public bool IsPathFound => Path != null && Path.Length > 0;
        public Coordinate[] Path { get; set; }
    }
}
