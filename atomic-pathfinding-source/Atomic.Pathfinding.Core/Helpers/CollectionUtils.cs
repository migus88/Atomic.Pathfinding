using System;
using Atomic.Pathfinding.Core.Data;

namespace Atomic.Pathfinding.Core.Helpers;

public static class CollectionUtils
{
    public static unsafe Coordinate* ToCoordinateArrayPointer(this ReadOnlySpan<Coordinate> span)
    {
        fixed (Coordinate* ptr = span)
        {
            return ptr;
        }
    }
}