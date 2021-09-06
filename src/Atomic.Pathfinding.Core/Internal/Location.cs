using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Data;

namespace Atomic.Pathfinding.Core.Internal
{
    internal class Location : IPriorityProvider
    {
        public bool IsClosed;
        
        public Coordinate Position { get; set; }
        public float ScoreF { get; set; }
        public float ScoreH { get; set; }
        public float ScoreG { get; set; }

        public Location Parent
        {
            get => _parent;
            set
            {
                _parent = value;
                Depth = _parent?.Depth + 1 ?? 1;
            }
        }
        public int Depth { get; private set; }

        private Location _parent;


        public int QueueIndex { get; set; }
        public float Priority
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
