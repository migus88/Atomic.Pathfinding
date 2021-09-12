using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Data;

namespace Atomic.Pathfinding.Core.Helpers
{
    public static class Utils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCellIndex(int x, int y, int width) => (y * width) + x;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCellIndex(Coordinate coordinate, int width) => (int)coordinate.Y * (int)width + (int)coordinate.X;
    }
}