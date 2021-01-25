using System.Collections.Generic;
using Atomic.Pathfinding.Core.Internal;

namespace Atomic.Pathfinding.Core.Helpers
{
    public static class PathHelpers
    {
        public static int X(this (int, int) vector)
        {
            return vector.Item1;
        }

        public static int Y(this (int, int) vector)
        {
            return vector.Item2;
        }

        public static void X(this (int, int) vector, int x)
        {
            vector.Item1 = x;
        }

        public static void Y(this (int, int) vector, int y)
        {
            vector.Item2 = y;
        }

        internal static Location GetLowestCostLocation(this Dictionary<(int, int), Location> dict)
        {
            Location result = null;

            foreach (var item in dict)
            {
                if (result == null || item.Value.ScoreF < result.ScoreF)
                {
                    result = item.Value;
                }
            }

            return result;
        }
    }
}
