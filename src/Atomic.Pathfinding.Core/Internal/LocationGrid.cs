using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Helpers;
using Atomic.Pathfinding.Core.Interfaces;

namespace Atomic.Pathfinding.Core.Internal
{
    internal class LocationGrid
    {
        private const int MaxNeighbors = 8;

        private const int LeftNeighborPosition = 0;
        private const int RightNeighborPosition = 1;
        private const int UpNeighborPosition = 2;
        private const int DownNeighborPosition = 3;
        private const int LeftDownNeighborPosition = 4;
        private const int LeftUpNeighborPosition = 5;
        private const int RightDownNeighborPosition = 6;
        private const int RightUpNeighborPosition = 7;
        
        public FastPriorityQueue<Location> OpenSet { get; private set; } = new FastPriorityQueue<Location>();

        public Location Current { get; set; } = null;

        private readonly Location[,] _matrix;
        private readonly IGrid _grid;
        private readonly PathfinderSettings _settings;

        private readonly int _width;
        private readonly int _height;

        private readonly Location[] _neighbors = new Location[MaxNeighbors];

        public LocationGrid(IGrid grid, PathfinderSettings settings)
        {
            _grid = grid;
            _settings = settings;

            _height = _grid.Matrix.GetLength(0);
            _width = _grid.Matrix.GetLength(1);

            _matrix = new Location[_height, _width];

            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    var location = new Location
                    {
                        Position = new Coordinate {X = x, Y = y}
                    };
                    _matrix[y, x] = location;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Location GetLocation(Coordinate position)
        {
            return !IsPositionValid(position.X, position.Y) ? null : _matrix[position.Y, position.X];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Location[] GetNeighbors(Coordinate position, int agentSize)
        {
            for (var i = 0; i < _neighbors.Length; i++)
            {
                _neighbors[i] = null;
            }

            _neighbors[LeftNeighborPosition] = GetWalkableLocation(position.X - 1, position.Y, agentSize);
            _neighbors[RightNeighborPosition] = GetWalkableLocation(position.X + 1, position.Y, agentSize);
            _neighbors[DownNeighborPosition] = GetWalkableLocation(position.X, position.Y + 1, agentSize);
            _neighbors[UpNeighborPosition] = GetWalkableLocation(position.X, position.Y - 1, agentSize);

            var canGoLeft = _neighbors[LeftNeighborPosition] != null;
            var canGoRight = _neighbors[RightNeighborPosition] != null;
            var canGoDown = _neighbors[DownNeighborPosition] != null;
            var canGoUp = _neighbors[UpNeighborPosition] != null;

            if (!_settings.IsDiagonalMovementEnabled)
            {
                return _neighbors;
            }
            
            if (canGoLeft || canGoDown || _settings.IsMovementBetweenCornersEnabled)
            {
                _neighbors[LeftDownNeighborPosition] = GetWalkableLocation(position.X - 1, position.Y + 1, agentSize);
            }

            if (canGoLeft || canGoUp || _settings.IsMovementBetweenCornersEnabled)
            {
                _neighbors[LeftUpNeighborPosition] = GetWalkableLocation(position.X - 1, position.Y - 1, agentSize);
            }

            if (canGoRight || canGoDown || _settings.IsMovementBetweenCornersEnabled)
            {
                _neighbors[RightDownNeighborPosition] = GetWalkableLocation(position.X + 1, position.Y + 1, agentSize);
            }

            if (canGoRight || canGoUp || _settings.IsMovementBetweenCornersEnabled)
            {
                _neighbors[RightUpNeighborPosition] = GetWalkableLocation(position.X + 1, position.Y - 1, agentSize);
            }

            return _neighbors;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            OpenSet.Clear();
            Current = null;

            foreach (var item in _matrix)
            {
                item?.Reset();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Location GetWalkableLocation(int x, int y)
        {
            if (!IsPositionValid(x, y))
                return null;

            var cell = _grid.Matrix[y, x];

            return (_settings.IsCalculatingOccupiedCells && cell.IsOccupied) || !cell.IsWalkable ? null : _matrix[y, x];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Location GetWalkableLocation(int x, int y, int agentSize)
        {
            var location = GetWalkableLocation(x, y);

            if (location == null)
            {
                return null;
            }

            //TODO: Implement clearance "baking" for non dinamyc grids
            //Clearance calculation
            for (var nY = 0; nY < agentSize; nY++)
            {
                for (var nX = 0; nX < agentSize; nX++)
                {
                    var neighbor = GetWalkableLocation(x + nX, y + nY);

                    if (neighbor == null)
                    {
                        return null;
                    }
                }
            }
            
            return location;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsPositionValid(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }
    }
}