using System;
using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Helpers;
using Atomic.Pathfinding.Core.Interfaces;
using Atomic.Pathfinding.Core.Internal;
using static Atomic.Pathfinding.Core.Helpers.DirectionIndexes;

namespace Atomic.Pathfinding.Core
{
    public sealed unsafe class TerrainPathfinder<T> where T : unmanaged, ICell
    {
        private const int MaxNeighbors = 8;

        private readonly PathfinderSettings _settings;
        private readonly int _width;
        private readonly int _height;

        private readonly Coordinate?[] _neighbors = new Coordinate?[MaxNeighbors];
        private readonly FasterPriorityQueue<T> _openSet;

        public TerrainPathfinder(int width, int height, PathfinderSettings settings = null)
        {
            _settings = settings ?? new PathfinderSettings();

            _width = width;
            _height = height;
            _openSet = new FasterPriorityQueue<T>(_width * _height); //TODO: Optimize the size (for example measure the amount of non walkable cells)
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PathResult GetPath(ICellProvider<T> provider, IAgent agent, Coordinate from, Coordinate to)
        {
            if (!IsPositionValid(to))
                throw new Exception("Destination is not valid");
            
            provider.ResetCells();
            _openSet.Clear();
            
            var h = GetH(from, to);

            var current = provider.GetCellPointer(from.X, from.Y);

            _openSet.Enqueue(current, h); //F set by the queue

            while (_openSet.Count > 0)
            {
                current = _openSet.Dequeue();

                if (current->Coordinate == to)
                {
                    break;
                }

                current->IsClosed = true;

                var neighboringCoordinates = GetNeighboringCoordinates(provider, current, agent.Size);

                foreach (var neighborCoordinate in neighboringCoordinates)
                {
                    if (neighborCoordinate == null)
                    {
                        continue;
                    }

                    var neighbor = provider.GetCellPointer(neighborCoordinate.Value);

                    if (neighbor->IsClosed)
                    {
                        continue;
                    }

                    h = GetH(neighbor->Coordinate, to);

                    var g = current->ScoreG +
                            h +
                            GetNeighborTravelWeight(current->Coordinate, neighbor->Coordinate) +
                            GetCellWeight(neighbor);

                    if (!_openSet.Contains(neighbor))
                    {
                        neighbor->ParentCoordinate = current->Coordinate;
                        neighbor->Depth = current->Depth + 1;
                        neighbor->ScoreG = g;
                        neighbor->ScoreH = h;

                        var f = g + h;

                        _openSet.Enqueue(neighbor, f); //F set by the queue
                    }
                    else if (g + neighbor->ScoreH < neighbor->ScoreF)
                    {
                        neighbor->ScoreG = g;
                        neighbor->ScoreF = g + neighbor->ScoreH;
                        neighbor->ParentCoordinate = current->Coordinate;
                        neighbor->Depth = current->Depth + 1;
                    }
                }
            }

            var result = new PathResult();

            if (current->Coordinate != to)
            {
                return result;
            }


            var last = current;
            var stack = new Coordinate[last->Depth];

            for (var i = last->Depth - 1; i >= 0; i--)
            {
                stack[i] = last->Coordinate;
                last = provider.GetCellPointer(last->ParentCoordinate);
            }

            result.Path = stack;

            return result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetNeighborTravelWeight(Coordinate start, Coordinate destination)
        {
            return IsDiagonalMovement(start, destination)
                ? _settings.DiagonalMovementCost
                : _settings.StraightMovementCost;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetCellWeight(T* cell)
        {
            return _settings.IsCellWeightEnabled ? cell->Weight : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Coordinate?[] GetNeighboringCoordinates(ICellProvider<T> provider, T* current, int agentSize)
        {
            for (var i = 0; i < _neighbors.Length; i++)
            {
                _neighbors[i] = null;
            }

            var position = current->Coordinate;

            _neighbors[West] = GetWalkableLocation(provider, position.X - 1, position.Y, agentSize);
            _neighbors[East] = GetWalkableLocation(provider, position.X + 1, position.Y, agentSize);
            _neighbors[South] = GetWalkableLocation(provider, position.X, position.Y + 1, agentSize);
            _neighbors[North] = GetWalkableLocation(provider, position.X, position.Y - 1, agentSize);

            if (!_settings.IsDiagonalMovementEnabled)
            {
                return _neighbors;
            }

            var canGoLeft = _neighbors[West] != null;
            var canGoRight = _neighbors[East] != null;
            var canGoDown = _neighbors[South] != null;
            var canGoUp = _neighbors[North] != null;

            if (canGoLeft || canGoDown || _settings.IsMovementBetweenCornersEnabled)
            {
                _neighbors[SouthWest] = GetWalkableLocation(provider, position.X - 1, position.Y + 1, agentSize);
            }

            if (canGoLeft || canGoUp || _settings.IsMovementBetweenCornersEnabled)
            {
                _neighbors[NorthWest] = GetWalkableLocation(provider, position.X - 1, position.Y - 1, agentSize);
            }

            if (canGoRight || canGoDown || _settings.IsMovementBetweenCornersEnabled)
            {
                _neighbors[SouthEast] = GetWalkableLocation(provider, position.X + 1, position.Y + 1, agentSize);
            }

            if (canGoRight || canGoUp || _settings.IsMovementBetweenCornersEnabled)
            {
                _neighbors[NorthEast] = GetWalkableLocation(provider, position.X + 1, position.Y - 1, agentSize);
            }

            return _neighbors;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Coordinate? GetWalkableLocation(ICellProvider<T> provider, float x, float y)
        {
            return GetWalkableLocation(provider, new Coordinate(x, y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Coordinate? GetWalkableLocation(ICellProvider<T> provider, Coordinate coordinate)
        {
            if (!IsPositionValid(coordinate))
                return null;

            var cell = provider.GetCellPointer(coordinate);

            return (_settings.IsCalculatingOccupiedCells && cell->IsOccupied) || !cell->IsWalkable
                ? (Coordinate?)null
                : cell->Coordinate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Coordinate? GetWalkableLocation(ICellProvider<T> provider, float x, float y, int agentSize)
        {
            return GetWalkableLocation(provider, new Coordinate(x, y), agentSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Coordinate? GetWalkableLocation(ICellProvider<T> provider, Coordinate coordinate, int agentSize)
        {
            var location = GetWalkableLocation(provider, coordinate);

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
                    var neighbor = GetWalkableLocation(provider, new Coordinate(coordinate.X + nX, coordinate.Y + nY));

                    if (neighbor == null)
                    {
                        return null;
                    }
                }
            }

            return location;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsPositionValid(Coordinate coordinate)
        {
            return coordinate.X >= 0 && coordinate.X < _width && coordinate.Y >= 0 && coordinate.Y < _height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetH(Coordinate start, Coordinate destination)
        {
            // return (start.X - destination.X) * (start.X - destination.X) +
            //        (start.Y - destination.Y) * (start.Y - destination.Y);

            return Math.Abs((float)destination.X - start.X) + Math.Abs((float)destination.Y - start.Y);
        }

        private bool IsDiagonalMovement(Coordinate start, Coordinate destination)
        {
            return start.X != destination.X && start.Y != destination.Y;
        }
    }
}