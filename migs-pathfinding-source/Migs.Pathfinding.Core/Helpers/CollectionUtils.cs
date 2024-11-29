using System;
using Migs.Pathfinding.Core.Data;

namespace Migs.Pathfinding.Core.Helpers;

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