using System;

namespace Atomic.Pathfinding.Core.Data
{
    public struct Coordinate : IEquatable<Coordinate>
    {
        private const int HashMultiplier = 397;
        
        public float X { get; set; }
        public float Y { get; set; }

        public Coordinate(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void Reset()
        {
            X = 0;
            Y = 0;
        }

        public static bool operator ==(Coordinate left, Coordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Coordinate left, Coordinate right)
        {
            return !(left == right);
        }

        public bool Equals(Coordinate other)
        {
            return Math.Abs(X - other.X) < 0.001f && Math.Abs(Y - other.Y) < 0.001f;
        }

        public override bool Equals(object obj)
        {
            return obj is Coordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * HashMultiplier) ^ Y.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"{X}:{Y}";
        }
    }
}