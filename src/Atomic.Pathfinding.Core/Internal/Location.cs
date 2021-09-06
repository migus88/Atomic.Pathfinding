using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Data;

namespace Atomic.Pathfinding.Core.Internal
{
    internal class Location : IPriorityProvider
    {
        public bool IsClosed { get; set; }
        
        public Coordinate Position { get; private set; }
        public double ScoreF { get; private set; }
        public double ScoreH { get; private set; }
        public double ScoreG { get; private set; }
        public Location Parent { get; private set; }


        public int QueueIndex { get; set; }
        public double Priority
        {
            get => ScoreF;
            set => SetScoreF(value);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPosition(Coordinate position)
        {
            Position = position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetScoreF(double scoreF)
        {
            ScoreF = scoreF;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetScoreH(double scoreH)
        {
            ScoreH = scoreH;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetScoreG(double scoreG)
        {
            ScoreG = scoreG;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetParent(Location parent)
        {
            Parent = parent;
        }
    }
}
