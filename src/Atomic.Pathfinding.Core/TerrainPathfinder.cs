using System;
using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Helpers;
using Atomic.Pathfinding.Core.Interfaces;
using Atomic.Pathfinding.Core.Internal;
using static Atomic.Pathfinding.Core.Helpers.DirectionIndexes;

namespace Atomic.Pathfinding.Core
{
    public sealed class TerrainPathfinder
    {
        private const int MaxNeighbors = 8;
        private const double MaxHScoreBetweenNeighbors = 2;
        private const double CostTolerance = 0.01;
        private const int IllegalIndex = -1;

        private readonly PathfinderSettings _settings;
        private readonly int _width;
        private readonly int _height;

        private readonly int[] _neighbors = new int[MaxNeighbors];

        public TerrainPathfinder(int width, int height, PathfinderSettings settings = null)
        {
            _settings = settings ?? new PathfinderSettings();

            _width = width;
            _height = height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PathResult GetPath(Cell[] cells, IAgent agent, Coordinate from, Coordinate to)
        {
            if (!IsPositionValid(to.X, to.Y))
                throw new Exception("Destination is not valid");
            
            var openSet =
                new FasterPriorityQueue(_width *
                                        _height); //TODO: Optimize the size (for example measure the amount of non walkable cells)

            var startIndex = Utils.GetCellIndex(from.X, from.Y, _width);
            var h = GetH(ref from, ref to);
            var current = new PriorityQueueItem(startIndex);

            openSet.Enqueue(current, h);
            cells[startIndex].SetF(h);

            while (openSet.Count > 0)
            {
                current = openSet.Dequeue();

                if (cells[current.CellIndex].Coordinate == to)
                {
                    break;
                }
                
                cells[current.CellIndex].SetIsClosed(true);
                cells[current.CellIndex].SetQueueItem(current);
                var currentCoordinate = cells[current.CellIndex].Coordinate;

                var neighborIndexes = GetNeighborIndexes(ref cells, ref currentCoordinate, agent.Size);

                foreach (var neighborIndex in neighborIndexes)
                {
                    if (neighborIndex == IllegalIndex || cells[neighborIndex].IsClosed)
                    {
                        continue;
                    }

                    var neighborCoordinate = cells[neighborIndex].Coordinate;

                    var g = cells[current.CellIndex].G
                            + GetNeighborTravelWeight(ref currentCoordinate, ref neighborCoordinate)
                            + GetCellWeight(ref cells, neighborIndex);

                    if (!openSet.Contains(cells[neighborIndex].QueueItem))
                    {
                        cells[neighborIndex].SetParentCoordinate(cells[current.CellIndex].Coordinate);
                        cells[neighborIndex].SetDepth(cells[current.CellIndex].Depth + 1);
                        cells[neighborIndex].SetG(g);

                        h = GetH(ref neighborCoordinate, ref to);
                        cells[neighborIndex].SetH(h);

                        var f = g + h;
                        cells[neighborIndex].SetF(f);

                        var newQueueItem = new PriorityQueueItem(neighborIndex);
                        newQueueItem.SetQueueIndex(openSet.Count + 1);
                        cells[neighborIndex].SetQueueItem(newQueueItem);
                        openSet.Enqueue(newQueueItem, f);
                    }
                    else if (g + cells[neighborIndex].H < cells[neighborIndex].F)
                    {
                        cells[neighborIndex].SetG(g);
                        cells[neighborIndex].SetF(g + cells[neighborIndex].H);
                        cells[neighborIndex].SetParentCoordinate(cells[current.CellIndex].Coordinate);
                        cells[neighborIndex].SetDepth(cells[current.CellIndex].Depth + 1);
                    }
                }
            }

            var result = new PathResult
            {
                // Cells = cells
            };

            if (cells[current.CellIndex].Coordinate != to)
            {
                return result;
            }


            var last = cells[current.CellIndex];
            var stack = new Coordinate[last.Depth];

            for (int i = last.Depth - 1; i >= 0; i--)
            {
                stack[i] = last.Coordinate;
                var parentIndex = Utils.GetCellIndex(last.ParentCoordinate.X, last.ParentCoordinate.Y, _width);
                last = cells[parentIndex];
            }

            result.Path = stack;

            return result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetNeighborTravelWeight(ref Coordinate start, ref Coordinate destination)
        {
            return IsDiagonalMovement(ref start, ref destination)
                ? _settings.DiagonalMovementCost
                : _settings.StraightMovementCost;

            var scoreH = GetH(ref start, ref destination);

            if (scoreH > MaxHScoreBetweenNeighbors)
                throw new Exception("Can travel only to neighbors");

            return Math.Abs(scoreH - _settings.StraightMovementCost) < CostTolerance
                ? _settings.StraightMovementCost
                : _settings.DiagonalMovementCost;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetCellWeight(ref Cell[] cells, int cellIndex)
        {
            return _settings.IsCellWeightEnabled ? cells[cellIndex].Weight : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int[] GetNeighborIndexes(ref Cell[] cells, ref Coordinate position, int agentSize)
        {
            for (var i = 0; i < _neighbors.Length; i++)
            {
                _neighbors[i] = IllegalIndex;
            }

            _neighbors[West] = GetWalkableLocationIndex(ref cells, position.X - 1, position.Y, agentSize);
            _neighbors[East] = GetWalkableLocationIndex(ref cells, position.X + 1, position.Y, agentSize);
            _neighbors[South] = GetWalkableLocationIndex(ref cells, position.X, position.Y + 1, agentSize);
            _neighbors[North] = GetWalkableLocationIndex(ref cells, position.X, position.Y - 1, agentSize);

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
                _neighbors[SouthWest] = GetWalkableLocationIndex(ref cells, position.X - 1, position.Y + 1, agentSize);
            }

            if (canGoLeft || canGoUp || _settings.IsMovementBetweenCornersEnabled)
            {
                _neighbors[NorthWest] = GetWalkableLocationIndex(ref cells, position.X - 1, position.Y - 1, agentSize);
            }

            if (canGoRight || canGoDown || _settings.IsMovementBetweenCornersEnabled)
            {
                _neighbors[SouthEast] = GetWalkableLocationIndex(ref cells, position.X + 1, position.Y + 1, agentSize);
            }

            if (canGoRight || canGoUp || _settings.IsMovementBetweenCornersEnabled)
            {
                _neighbors[NorthEast] = GetWalkableLocationIndex(ref cells, position.X + 1, position.Y - 1, agentSize);
            }

            return _neighbors;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetWalkableLocationIndex(ref Cell[] cells, int x, int y)
        {
            if (!IsPositionValid(x, y))
                return IllegalIndex;

            var cellIndex = Utils.GetCellIndex(x, y, _width);
            var cell = cells[cellIndex];

            return (_settings.IsCalculatingOccupiedCells && cell.IsOccupied) || !cell.IsWalkable
                ? IllegalIndex
                : cellIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetWalkableLocationIndex(ref Cell[] cells, int x, int y, int agentSize)
        {
            var location = GetWalkableLocationIndex(ref cells, x, y);

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
                    var neighbor = GetWalkableLocationIndex(ref cells, x + nX, y + nY);

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
        private float GetH(ref Coordinate start, ref Coordinate destination)
        {
            // return (start.X - destination.X) * (start.X - destination.X) +
            //        (start.Y - destination.Y) * (start.Y - destination.Y);
            
            return Math.Abs((float) destination.X - start.X) + Math.Abs((float) destination.Y - start.Y);
        }

        private bool IsDiagonalMovement(ref Coordinate start, ref Coordinate destination)
        {
            return start.X != destination.X && start.Y != destination.Y;
        }
    }
}