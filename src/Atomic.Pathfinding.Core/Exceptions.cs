using System;
using Atomic.Pathfinding.Core.Data;

namespace Atomic.Pathfinding.Core
{
    public class LocationNotFoundException : Exception
    {
        public LocationNotFoundException(Coordinate coordinate) : base($"Location not found: {coordinate}") { }
    }

    public class EmptyGridException : Exception
    {
        public EmptyGridException() : base("Grid cannot be empty or null") { }
    }

    public class LocationGridUnreachableException : Exception
    {
        public LocationGridUnreachableException() : base(
            "Can't get or create a location grid. This can be caused by poor threading implementation") { }
    
    }

}
