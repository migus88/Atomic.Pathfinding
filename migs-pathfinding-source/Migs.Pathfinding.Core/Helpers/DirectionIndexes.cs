using System.Collections.Generic;
using Migs.Pathfinding.Core.Data;

namespace Migs.Pathfinding.Core.Helpers
{
    public static class DirectionIndexes
    {
        public const int West = 0;
        public const int East = 1;
        public const int North = 2;
        public const int South = 3;
        public const int SouthWest = 4;
        public const int NorthWest = 5;
        public const int SouthEast = 6;
        public const int NorthEast = 7;

        public const int DiagonalStart = SouthWest;
    }
}