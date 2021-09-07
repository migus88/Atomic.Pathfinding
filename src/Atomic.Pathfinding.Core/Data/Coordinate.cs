using System;

namespace Atomic.Pathfinding.Core.Data
{
    public readonly struct Coordinate : IEquatable<Coordinate>
    {
        private const int HashMultiplier = 397;
        
        public readonly short X { get; }
        public readonly short Y { get; }

        public Coordinate(short x, short y)
        {
            X = x;
            Y = y;
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
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Coordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * HashMultiplier) ^ Y;
            }
        }

        public override string ToString()
        {
            return $"{X}:{Y}";
        }
    }
}