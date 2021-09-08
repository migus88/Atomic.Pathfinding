using System.Collections.Generic;
using System.Linq;

namespace Atomic.Pathfinding.Core.Data
{
    public struct PathResult
    {
        public bool IsPathFound => Path != null && Path.Any();
        public IEnumerable<Coordinate> Path { get; set; }
        // public Cell[] Cells { get; set; }
    }
}
