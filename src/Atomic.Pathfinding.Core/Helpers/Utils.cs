using System.Runtime.CompilerServices;

namespace Atomic.Pathfinding.Core.Helpers
{
    public static class Utils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCellIndex(int x, int y, int height) => (y * height) + x;
    }
}