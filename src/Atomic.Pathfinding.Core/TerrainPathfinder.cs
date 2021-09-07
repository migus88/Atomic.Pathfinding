using System;
using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Interfaces;
using Atomic.Pathfinding.Core.Internal;
using Atomic.Pathfinding.Core.Terrain;
using static Atomic.Pathfinding.Core.Helpers.DirectionIndexes;

namespace Atomic.Pathfinding.Core
{
    public class TerrainPathfinder
    {
        private const int MaxNeighbors = 8;
        private const double MaxHScoreBetweenNeighbors = 2;
        private const double CostTolerance = 0.01;
        private const int WidthDimension = 0;
        private const int HeightDimension = 1;
        private const int IllegalIndex = -1;
        
        private readonly IGrid _grid;
        private readonly PathfinderSettings _settings;
        private readonly int _width;
        private readonly int _height;
        
        private readonly int[] _neighbors = new int[MaxNeighbors];

        public TerrainPathfinder(IGrid grid, PathfinderSettings settings = null)
        {
            if (grid?.Matrix == null || grid.Matrix.Length == 0)
                throw new EmptyGridException();
            
            _settings = settings ?? new PathfinderSettings();
            _grid = grid;

            _width = _grid.Matrix.GetLength(WidthDimension);
            _height = _grid.Matrix.GetLength(HeightDimension);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PathResult GetPath(IAgent agent, Coordinate from, Coordinate to)
        {
            Span<Cell> cells = stackalloc Cell[_width * _height];

            for (short x = 0; x < _width; x++)
            {
                for (short y = 0; y < _height; y++)
                {
                    var index = GetCellIndex(x, y);
                    cells[index].SetCoordinate(new Coordinate(x,y));
                    cells[index].SetQueueItem(new PriorityQueueItem(index));
                }
            }
            
            var openSet = new FasterPriorityQueue(_width * _height); //TODO: Optimize the size (for example measure the amount of non walkable cells)

            var startIndex = GetCellIndex(from.X, from.Y);
            var h = GetH(ref from, ref to);
            var current = new PriorityQueueItem(startIndex);
            
            openSet.Enqueue(ref current, h);

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
                
                var neighborIndexes = GetNeighborIndexes(ref currentCoordinate, agent.Size);

                foreach (var neighborIndex in neighborIndexes)
                {
                    if (neighborIndex == IllegalIndex || cells[neighborIndex].IsClosed)
                    {
                        continue;
                    }

                    var neighborCoordinate = cells[current.CellIndex].Coordinate;
                    
                    var g = cells[neighborIndex].G
                            + GetNeighborTravelWeight(ref currentCoordinate, ref neighborCoordinate)
                            + GetCellWeight( ref neighborCoordinate);

                    if (!openSet.Contains(cells[neighborIndex].QueueItem))
                    {
                        cells[neighborIndex].SetParentCoordinate(cells[current.CellIndex].Coordinate);
                        cells[neighborIndex].SetDepth(cells[current.CellIndex].Depth + 1);
                        cells[neighborIndex].SetG(g);

                        h = GetH(ref neighborCoordinate, ref to);
                        cells[neighborIndex].SetH(h);

                        var f = g + h;

                        var newQueueItem = new PriorityQueueItem(neighborIndex);
                        cells[neighborIndex].SetQueueItem(newQueueItem);
                        openSet.Enqueue(ref newQueueItem, f);
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

            var result = new PathResult();

            if (cells[current.CellIndex].Coordinate != to)
            {
                return result;
            }
            
            
            var last = cells[current.CellIndex];
            var stack = new Coordinate[last.Depth];

            for (int i = last.Depth - 1; i >= 0; i--)
            {
                stack[i] = last.Coordinate;
                var parentIndex = GetCellIndex(last.ParentCoordinate.X, last.ParentCoordinate.Y);
                last = cells[parentIndex];
            }

            result.Path = stack;

            return result;
        }
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetNeighborTravelWeight(ref Coordinate start, ref Coordinate destination)
        {
            var scoreH = GetH(ref start, ref destination);

            if (scoreH > MaxHScoreBetweenNeighbors)
                throw new Exception("Can travel only to neighbors");

            return Math.Abs(scoreH - _settings.StraightMovementCost) < CostTolerance
                ? _settings.StraightMovementCost
                : _settings.DiagonalMovementCost;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetCellWeight(ref Coordinate position)
        {
            return _settings.IsCellWeightEnabled ? _grid.Matrix[position.Y, position.X].Weight : 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int[] GetNeighborIndexes(ref Coordinate position, int agentSize)
        {
            for (var i = 0; i < _neighbors.Length; i++)
            {
                _neighbors[i] = IllegalIndex;
            }

            _neighbors[West] = GetWalkableLocationIndex(position.X - 1, position.Y, agentSize);
            _neighbors[East] = GetWalkableLocationIndex(position.X + 1, position.Y, agentSize);
            _neighbors[South] = GetWalkableLocationIndex(position.X, position.Y + 1, agentSize);
            _neighbors[North] = GetWalkableLocationIndex(position.X, position.Y - 1, agentSize);

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
                _neighbors[SouthWest] = GetWalkableLocationIndex(position.X - 1, position.Y + 1, agentSize);
            }

            if (canGoLeft || canGoUp || _settings.IsMovementBetweenCornersEnabled)
            {
                _neighbors[NorthWest] = GetWalkableLocationIndex(position.X - 1, position.Y - 1, agentSize);
            }

            if (canGoRight || canGoDown || _settings.IsMovementBetweenCornersEnabled)
            {
                _neighbors[SouthEast] = GetWalkableLocationIndex(position.X + 1, position.Y + 1, agentSize);
            }

            if (canGoRight || canGoUp || _settings.IsMovementBetweenCornersEnabled)
            {
                _neighbors[NorthEast] = GetWalkableLocationIndex(position.X + 1, position.Y - 1, agentSize);
            }

            return _neighbors;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetWalkableLocationIndex(int x, int y)
        {
            if (!IsPositionValid(x, y))
                return IllegalIndex;

            var cell = _grid.Matrix[y, x];

            return (_settings.IsCalculatingOccupiedCells && cell.IsOccupied) || !cell.IsWalkable ? IllegalIndex : GetCellIndex(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetWalkableLocationIndex(int x, int y, int agentSize)
        {
            var location = GetWalkableLocationIndex(x, y);

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
                    var neighbor = GetWalkableLocationIndex(x + nX, y + nY);

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
        private int GetH(ref Coordinate start, ref Coordinate destination)
        {
            return Math.Abs(destination.X - start.X) + Math.Abs(destination.Y - start.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetCellIndex(int x, int y)
        {
            return (x * _height) + y;
        }
    }
}