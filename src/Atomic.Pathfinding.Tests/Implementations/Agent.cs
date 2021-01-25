using System;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Interfaces;

namespace Atomic.Pathfinding.Tests.Implementations
{
    public class Agent : IAgent
    {
        public int Size { get; set; } = 1;
    
        public Action<PathResult> PathResultAction { get; set; }

        public void OnPathResult(PathResult result)
        {
            PathResultAction?.Invoke(result);
        }
    }
}
