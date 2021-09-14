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

        private readonly FasterPriorityQueue<T> _openSet;

        public TerrainPathfinder(int width, int height, PathfinderSettings settings = null)
        {
            _settings = settings ?? new PathfinderSettings();

            _width = width;
            _height = height;
            _openSet = new FasterPriorityQueue<T>(_width *
                                                  _height); //TODO: Optimize the size (for example measure the amount of non walkable cells)
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PathResult GetPath(ICellProvider<T> provider, IAgent agent, Coordinate from, Coordinate to)
        {
            if (!IsPositionValid(to.X, to.Y))
                throw new Exception("Destination is not valid");

            provider.ResetCells();
            _openSet.Clear();

            var h = GetH(from.X, from.Y, to.X, to.Y);

            var current = provider.GetCellPointer(from.X, from.Y);
            _openSet.Enqueue(current, h); //F set by the queue
            var neighbors = new T*[MaxNeighbors];

            while (_openSet.Count > 0)
            {
                current = _openSet.Dequeue();

                if (current->Coordinate == to)
                {
                    break;
                }

                current->IsClosed = true;

                GetNeighbors(ref provider, current, agent.Size, ref neighbors);

                foreach (var neighbor in neighbors)
                {
                    if (neighbor == null || neighbor->IsClosed)
                    {
                        continue;
                    }

                    h = GetH(neighbor->Coordinate.X, neighbor->Coordinate.Y, to.X, to.Y);

                    var g = current->ScoreG +
                            h +
                            GetNeighborTravelWeight(current->Coordinate.X, current->Coordinate.Y,
                                neighbor->Coordinate.X, neighbor->Coordinate.Y) +
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
                last = provider.GetCellPointer(last->ParentCoordinate.X, last->ParentCoordinate.Y);
            }

            result.Path = stack;

            return result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetNeighborTravelWeight(int startX, int startY, int destX, int destY)
        {
            return IsDiagonalMovement(startX, startY, destX, destY)
                ? _settings.DiagonalMovementCost
                : _settings.StraightMovementCost;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetCellWeight(T* cell)
        {
            return _settings.IsCellWeightEnabled ? cell->Weight : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetNeighbors(ref ICellProvider<T> provider, T* current, int agentSize, ref T*[] neighbors)
        {
            for (var i = 0; i < neighbors.Length; i++)
            {
                neighbors[i] = default;
            }

            var position = current->Coordinate;

            neighbors[West] = GetWalkableLocation(ref provider, position.X - 1, position.Y, agentSize);
            neighbors[East] = GetWalkableLocation(ref provider, position.X + 1, position.Y, agentSize);
            neighbors[South] = GetWalkableLocation(ref provider, position.X, position.Y + 1, agentSize);
            neighbors[North] = GetWalkableLocation(ref provider, position.X, position.Y - 1, agentSize);

            if (!_settings.IsDiagonalMovementEnabled)
            {
                return;
            }

            var canGoLeft = neighbors[West] != null;
            var canGoRight = neighbors[East] != null;
            var canGoDown = neighbors[South] != null;
            var canGoUp = neighbors[North] != null;

            if (canGoLeft || canGoDown || _settings.IsMovementBetweenCornersEnabled)
            {
                neighbors[SouthWest] = GetWalkableLocation(ref provider, position.X - 1, position.Y + 1, agentSize);
            }

            if (canGoLeft || canGoUp || _settings.IsMovementBetweenCornersEnabled)
            {
                neighbors[NorthWest] = GetWalkableLocation(ref provider, position.X - 1, position.Y - 1, agentSize);
            }

            if (canGoRight || canGoDown || _settings.IsMovementBetweenCornersEnabled)
            {
                neighbors[SouthEast] = GetWalkableLocation(ref provider, position.X + 1, position.Y + 1, agentSize);
            }

            if (canGoRight || canGoUp || _settings.IsMovementBetweenCornersEnabled)
            {
                neighbors[NorthEast] = GetWalkableLocation(ref provider, position.X + 1, position.Y - 1, agentSize);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T* GetWalkableLocation(ref ICellProvider<T> provider, int x, int y)
        {
            if (!IsPositionValid(x, y))
                return default;

            var cell = provider.GetCellPointer(x, y);

            return (_settings.IsCalculatingOccupiedCells && cell->IsOccupied) || !cell->IsWalkable
                ? null
                : cell;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T* GetWalkableLocation(ref ICellProvider<T> provider, int x, int y, int agentSize)
        {
            var location = GetWalkableLocation(ref provider, x, y);

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
                    var neighbor = GetWalkableLocation(ref provider, x + nX, y + nY);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetH(int startX, int startY, int destX, int destY)
        {
            return Math.Abs(destX - startX) + Math.Abs(destY - startY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsDiagonalMovement(int startX, int startY, int destX, int destY)
        {
            return startX != destX && startY != destY;
        }
    }
}