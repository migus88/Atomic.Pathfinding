using System.Collections.Generic;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Internal;

namespace Atomic.Pathfinding.Core.Helpers
{
    public static class PathHelpers
    {

        internal static Location GetLowestCostLocation(this Dictionary<Coordinate, Location> dict)
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
