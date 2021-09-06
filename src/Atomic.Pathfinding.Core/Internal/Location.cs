using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Data;

namespace Atomic.Pathfinding.Core.Internal
{
    internal class Location : IPriorityProvider
    {
        public bool IsClosed { get; set; }
        
        public Coordinate Position { get; }
        public float ScoreF { get; set; }
        public float ScoreH { get; set; }
        public float ScoreG { get; set; }
        public int Depth { get; private set; }

        public int QueueIndex { get; set; }
        public float Priority
        {
            get => ScoreF;
            set => ScoreF = value;
        }

        public Location Parent
        {
            get => _parent;
            set
            {
                _parent = value;
                Depth = _parent?.Depth + 1 ?? 1;
            }
        }
        
        
        private Location _parent;

        public Location(Coordinate position)
        {
            Position = position;
        }

        public Location(short x, short y)
        {
            Position = new Coordinate(x, y);
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
