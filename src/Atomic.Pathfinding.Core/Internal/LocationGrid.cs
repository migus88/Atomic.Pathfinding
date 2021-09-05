using System.Collections.Generic;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Helpers;
using Atomic.Pathfinding.Core.Interfaces;

namespace Atomic.Pathfinding.Core.Internal
{
    internal class LocationGrid
    {
        public Dictionary<Coordinate, Location> OpenSet { get; private set; } = new Dictionary<Coordinate, Location>();

        public Dictionary<Coordinate, Location> ClosedSet { get; private set; } =
            new Dictionary<Coordinate, Location>();

        public Location Current { get; set; } = null;

        private readonly Location[,] _matrix;
        private readonly IGrid _grid;
        private readonly PathfinderSettings _settings;

        private readonly int _width;
        private readonly int _height;

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
                    _matrix[y, x] = new Location { Position = new Coordinate {X = x, Y = y} };
                }
            }
        }

        public Location GetLocation(Coordinate position)
        {
            return GetLocation(position.X, position.Y);
        }


        //TODO: This method creates garbage. Reuse the result.
        public List<Location> GetNeighbors(Coordinate position, int agentSize)
        {
            var list = new List<Location>(_settings.IsDiagonalMovementEnabled ? 8 : 4);

            var canGoLeft = PopulateWalkableLocation(ref list, position.X - 1, position.Y, agentSize);
            var canGoRight = PopulateWalkableLocation(ref list, position.X + 1, position.Y, agentSize);
            var canGoDown = PopulateWalkableLocation(ref list, position.X, position.Y + 1, agentSize);
            var canGoUp = PopulateWalkableLocation(ref list, position.X, position.Y - 1, agentSize);

            if (_settings.IsDiagonalMovementEnabled)
            {
                if (canGoLeft || canGoDown || _settings.IsMovementBetweenCornersEnabled)
                    PopulateWalkableLocation(ref list, position.X - 1, position.Y + 1, agentSize);

                if (canGoLeft || canGoUp || _settings.IsMovementBetweenCornersEnabled)
                    PopulateWalkableLocation(ref list, position.X - 1, position.Y - 1, agentSize);

                if (canGoRight || canGoDown || _settings.IsMovementBetweenCornersEnabled)
                    PopulateWalkableLocation(ref list, position.X + 1, position.Y + 1, agentSize);

                if (canGoRight || canGoUp || _settings.IsMovementBetweenCornersEnabled)
                    PopulateWalkableLocation(ref list, position.X + 1, position.Y - 1, agentSize);
            }

            return list;
        }

        public void Reset()
        {
            OpenSet.Clear();
            ClosedSet.Clear();
            Current = null;

            foreach (var item in _matrix)
            {
                item.Reset();
            }
        }

        private Location GetLocation(int x, int y)
        {
            if (!IsPositionValid(x, y))
                return null;

            return _matrix[y, x];
        }

        private Location GetWalkableLocation(int x, int y)
        {
            if (!IsPositionValid(x, y))
                return null;

            var cell = _grid.Matrix[y, x];

            return (_settings.IsCalculatingOccupiedCells && cell.IsOccupied) || !cell.IsWalkable ? null : _matrix[y, x];
        }

        private bool PopulateWalkableLocation(ref List<Location> locations, int x, int y, int agentSize)
        {
            var location = GetWalkableLocation(x, y);

            if (location == null)
            {
                return false;
            }

            //TODO: Implement clearance "baking" for non dinamyc grids
            //Clearance calculation
            for (int nY = 0; nY < agentSize; nY++)
            {
                for (int nX = 0; nX < agentSize; nX++)
                {
                    var neighbor = GetWalkableLocation(x + nX, y + nY);

                    if (neighbor == null)
                    {
                        return false;
                    }
                }
            }

            locations.Add(location);
            return true;
        }

        private bool IsPositionValid(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }
    }
}