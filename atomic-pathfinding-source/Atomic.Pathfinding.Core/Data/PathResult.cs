using System.Collections.Generic;
using System.Linq;

namespace Atomic.Pathfinding.Core.Data
{
    public struct PathResult
    {
        public bool IsPathFound => Path != null && Path.Any();
        public Coordinate[] Path { get; set; }
    }
}
