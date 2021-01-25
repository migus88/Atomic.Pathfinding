namespace Atomic.Pathfinding.Core.Internal
{
    internal class Location
    {
        public (int, int) Position { get; set; }
        public double ScoreF { get; set; }
        public double ScoreH { get; set; }
        public double ScoreG { get; set; }
        public Location Parent { get; set; }

        public void Reset()
        {
            ScoreF = 0;
            ScoreG = 0;
            ScoreH = 0;
            Parent = null;
        }
    }
}
