using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Data;

namespace Atomic.Pathfinding.Core.Internal
{
    internal class Location : IPriorityProvider
    {
        public bool IsClosed;
        
        public Coordinate Position { get; set; }
        public double ScoreF { get; set; }
        public double ScoreH { get; set; }
        public double ScoreG { get; set; }
        public Location Parent { get; set; }


        public int QueueIndex { get; set; }
        public double Priority
        {
            get => ScoreF;
            set => ScoreF = value;
        }
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            ScoreF = 0;
            ScoreG = 0;
            ScoreH = 0;
            Parent = null;
            IsClosed = false;
        }
    }
}
