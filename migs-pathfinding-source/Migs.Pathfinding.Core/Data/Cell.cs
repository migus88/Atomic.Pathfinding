using System.Runtime.CompilerServices;
using Migs.Pathfinding.Core.Interfaces;
using Migs.Pathfinding.Core.Data;
using Migs.Pathfinding.Core.Helpers;
using Migs.Pathfinding.Core.Internal;

namespace Migs.Pathfinding.Core.Data
{
    public struct Cell : ICell
    {
        public Coordinate Coordinate { get; set; }
        public bool IsWalkable { get; set; }
        public bool IsOccupied { get; set; }
        public float Weight { get; set; }
        
        internal bool IsClosed { get; set; }
        internal float ScoreF { get; set; }
        internal float ScoreH { get; set; }
        internal float ScoreG { get; set; }
        internal int Depth { get; set; }
        internal Coordinate ParentCoordinate { get; set; }
        internal int QueueIndex { get; set; }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            IsClosed = false;
            ScoreF = 0;
            ScoreH = 0;
            ScoreG = 0;
            Depth = 0;
            ParentCoordinate.Reset();
        }
    }
}