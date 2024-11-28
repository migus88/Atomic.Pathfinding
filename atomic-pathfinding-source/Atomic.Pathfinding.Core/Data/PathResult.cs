using System;
using System.Collections.Generic;
using System.Linq;

namespace Atomic.Pathfinding.Core.Data
{
    public ref struct PathResult
    {
        public bool IsPathFound => Path != null && Path.Length > 0;
        public ReadOnlySpan<Coordinate> Path { get; set; }
    }
}
