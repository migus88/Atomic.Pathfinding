using System;
using System.Runtime.CompilerServices;

namespace Atomic.Pathfinding.Core.Data
{
    public struct Coordinate : IEquatable<Coordinate>
    {
        private const int HashMultiplier = 397;
        
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsInitialized { get; set; }

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
            IsInitialized = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            X = 0;
            Y = 0;
            IsInitialized = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Coordinate left, Coordinate right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Coordinate left, Coordinate right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Coordinate other)
        {
            return Math.Abs(X - other.X) < 0.001f && Math.Abs(Y - other.Y) < 0.001f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is Coordinate other && Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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