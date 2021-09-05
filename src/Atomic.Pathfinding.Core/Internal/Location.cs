using Atomic.Pathfinding.Core.Data;

namespace Atomic.Pathfinding.Core.Internal
{
    internal class Location
    {
        public bool IsClosed { get; set; }
        
        public Coordinate Position { get; private set; }
        public double ScoreF { get; private set; }
        public double ScoreH { get; private set; }
        public double ScoreG { get; private set; }
        public Location Parent { get; private set; }

        public void Reset()
        {
            ScoreF = 0;
            ScoreG = 0;
            ScoreH = 0;
            Parent = null;
            IsClosed = false;
        }

        public void SetPosition(Coordinate position)
        {
            Position = position;
        }

        public void SetScoreF(double scoreF)
        {
            ScoreF = scoreF;
        }

        public void SetScoreH(double scoreH)
        {
            ScoreH = scoreH;
        }

        public void SetScoreG(double scoreG)
        {
            ScoreG = scoreG;
        }

        public void SetParent(Location parent)
        {
            Parent = parent;
        }
    }
}
