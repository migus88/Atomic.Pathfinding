using System.Collections.Generic;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Internal;

namespace Atomic.Pathfinding.Core.Helpers
{
    public static class PathHelpers
    {

        internal static Location GetLowestCostLocation(this HashSet<Location> dict)
        {
            Location result = null;

            foreach (var item in dict)
            {
                if (result == null || item.ScoreF < result.ScoreF)
                {
                    result = item;
                }
            }

            return result;
        }
    }
}
