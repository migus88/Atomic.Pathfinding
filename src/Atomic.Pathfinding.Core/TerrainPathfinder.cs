using System;
using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Helpers;
using Atomic.Pathfinding.Core.Interfaces;
using Atomic.Pathfinding.Core.Internal;
using static Atomic.Pathfinding.Core.Helpers.DirectionIndexes;

namespace Atomic.Pathfinding.Core
{
    public sealed unsafe class TerrainPathfinder
    {
        private const int MaxNeighbors = 8;
        private const double MaxHScoreBetweenNeighbors = 2;
        private const double CostTolerance = 0.01;
        private const int IllegalIndex = -1;

        private readonly PathfinderSettings _settings;
        private readonly int _width;
        private readonly int _height;

        private readonly int[] _neighbors = new int[MaxNeighbors];
        private readonly FasterPriorityQueue _openSet;

        public TerrainPathfinder(int width, int height, PathfinderSettings settings = null)
        {
            _settings = settings ?? new PathfinderSettings();

            _width = width;
            _height = height;
            _openSet = new FasterPriorityQueue(_width *  _height); //TODO: Optimize the size (for example measure the amount of non walkable cells)
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PathResult GetPath(Cell[] cells, IAgent agent, Coordinate from, Coordinate to)
        {
            if (!IsPositionValid(to.X, to.Y))
                throw new Exception("Destination is not valid");

            fixed(Cell *ptr = cells)
            {
                for (var i = 0; i < cells.Length; i++)
                {
                    cells[i].Reset();
                }

                _openSet.Clear();

                var startIndex = Utils.GetCellIndex(from.X, from.Y, _width);
                var h = GetH(from, to);

                var current = ptr + startIndex;

                _openSet.Enqueue(current, h); //F set by the queue

                while (_openSet.Count > 0)
                {
                    current = _openSet.Dequeue();

                    if (current->Coordinate == to)
                    {
                        break;
                    }

                    current->IsClosed = true;

                    var neighborIndexes = GetNeighborIndexes(ptr, current, agent.Size);

                    foreach (var neighborIndex in neighborIndexes)
                    {
                        if (neighborIndex == IllegalIndex)
                        {
                            continue;
                        }

                        var neighbor = ptr + neighborIndex;;

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
                    var parentIndex = Utils.GetCellIndex(last->ParentCoordinate.X, last->ParentCoordinate.Y, _width);
                    last = ptr + parentIndex;
                }

                result.Path = stack;

                return result;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetNeighborTravelWeight(Coordinate start, Coordinate destination)
        {
            return IsDiagonalMovement(start, destination)
                ? _settings.DiagonalMovementCost
                : _settings.StraightMovementCost;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetCellWeight(Cell* cell)
        {
            return _settings.IsCellWeightEnabled ? cell->Weight : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int[] GetNeighborIndexes(Cell* arrStart, Cell* current, int agentSize)
        {
            for (var i = 0; i < _neighbors.Length; i++)
            {
                _neighbors[i] = IllegalIndex;
            }

            var position = current->Coordinate;

            _neighbors[West] = GetWalkableLocationIndex(arrStart, position.X - 1, position.Y, agentSize);
            _neighbors[East] = GetWalkableLocationIndex(arrStart, position.X + 1, position.Y, agentSize);
            _neighbors[South] = GetWalkableLocationIndex(arrStart, position.X, position.Y + 1, agentSize);
            _neighbors[North] = GetWalkableLocationIndex(arrStart, position.X, position.Y - 1, agentSize);

            var canGoLeft = _neighbors[West] != IllegalIndex;
            var canGoRight = _neighbors[East] != IllegalIndex;
            var canGoDown = _neighbors[South] != IllegalIndex;
            var canGoUp = _neighbors[North] != IllegalIndex;

            if (!_settings.IsDiagonalMovementEnabled)
            {
                return _neighbors;
            }

            if (canGoLeft || canGoDown || _settings.IsMovementBetweenCornersEnabled)
            {
                _neighbors[SouthWest] = GetWalkableLocationIndex(arrStart, position.X - 1, position.Y + 1, agentSize);
            }

            if (canGoLeft || canGoUp || _settings.IsMovementBetweenCornersEnabled)
            {
                _neighbors[NorthWest] = GetWalkableLocationIndex(arrStart, position.X - 1, position.Y - 1, agentSize);
            }

            if (canGoRight || canGoDown || _settings.IsMovementBetweenCornersEnabled)
            {
                _neighbors[SouthEast] = GetWalkableLocationIndex(arrStart, position.X + 1, position.Y + 1, agentSize);
            }

            if (canGoRight || canGoUp || _settings.IsMovementBetweenCornersEnabled)
            {
                _neighbors[NorthEast] = GetWalkableLocationIndex(arrStart, position.X + 1, position.Y - 1, agentSize);
            }

            return _neighbors;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetWalkableLocationIndex(Cell* arrStart, int x, int y)
        {
            if (!IsPositionValid(x, y))
                return IllegalIndex;

            var cellIndex = Utils.GetCellIndex(x, y, _width);
            var cell = arrStart + cellIndex;

            return (_settings.IsCalculatingOccupiedCells && cell->IsOccupied) || !cell->IsWalkable
                ? IllegalIndex
                : cellIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetWalkableLocationIndex(Cell* arrStart, int x, int y, int agentSize)
        {
            var location = GetWalkableLocationIndex(arrStart, x, y);

            if (location == IllegalIndex)
            {
                return IllegalIndex;
            }

            //TODO: Implement clearance "baking" for non dinamyc grids
            //Clearance calculation
            for (var nY = 0; nY < agentSize; nY++)
            {
                for (var nX = 0; nX < agentSize; nX++)
                {
                    var neighbor = GetWalkableLocationIndex(arrStart, x + nX, y + nY);

                    if (neighbor == IllegalIndex)
                    {
                        return IllegalIndex;
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
        private float GetH(Coordinate start, Coordinate destination)
        {
            // return (start.X - destination.X) * (start.X - destination.X) +
            //        (start.Y - destination.Y) * (start.Y - destination.Y);

            return Math.Abs((float) destination.X - start.X) + Math.Abs((float) destination.Y - start.Y);
        }

        private bool IsDiagonalMovement(Coordinate start, Coordinate destination)
        {
            return start.X != destination.X && start.Y != destination.Y;
        }
    }
}