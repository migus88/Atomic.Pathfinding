using System;

namespace Atomic.Pathfinding.Core.Data
{
    public struct Coordinate : IEquatable<Coordinate>
    {
        private const int HashMultiplier = 397;
        
        public short X { get; private set; }
        public short Y { get; private set; }

        public Coordinate(short x, short y)
        {
            X = x;
            Y = y;
        }

        public void SetX(short x)
        {
            X = x;
        }

        public void SetY(short y)
        {
            Y = y;
        }

        public void Reset()
        {
            SetX(0);
            SetY(0);
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