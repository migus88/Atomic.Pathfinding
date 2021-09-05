using System.Collections.Generic;

namespace Atomic.Pathfinding.Core.Data
{
    public struct PathResult
    {
        public bool IsPathFound => Path != null && Path.Count > 0;
        public List<Coordinate> Path { get; set; }
    }
}
