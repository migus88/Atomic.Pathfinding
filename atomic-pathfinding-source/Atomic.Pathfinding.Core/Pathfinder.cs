using System;
using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Interfaces;
using Atomic.Pathfinding.Core.Internal;
using static Atomic.Pathfinding.Core.Helpers.DirectionIndexes;

namespace Atomic.Pathfinding.Core
{
    public sealed unsafe class Pathfinder
    {
        private const int MaxNeighbors = 8;
        private const int SingleCellAgentSize = 1;

        private readonly PathfinderSettings _settings;
        private readonly ICellProvider _cellProvider;

        private readonly FastPriorityQueue _openSet;

        public Pathfinder(ICellProvider cellProvider, PathfinderSettings settings = null)
        {
            _cellProvider = cellProvider;
            _settings = settings ?? new PathfinderSettings();

            _openSet = new FastPriorityQueue(_cellProvider.Width * _cellProvider.Height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PathResult GetPath(IAgent agent, Coordinate from, Coordinate to)
        {
            if (!IsPositionValid(to.X, to.Y))
                throw new Exception("Destination is not valid");

            _cellProvider.ResetCells();
            _openSet.Clear();

            var scoreH = GetH(from.X, from.Y, to.X, to.Y);

            var current = _cellProvider.GetCellPointer(from.X, from.Y);
            _openSet.Enqueue(current, scoreH); //ScoreF set by the queue

            var neighbors = new Cell*[MaxNeighbors];

            while (_openSet.Count > 0)
            {
                current = _openSet.Dequeue();

                if (current->Coordinate == to)
                {
                    break;
                }

                current->IsClosed = true;

                PopulateNeighbors(current, agent.Size, ref neighbors);

                foreach (var neighborPtr in neighbors)
                {
                    var neighbor = neighborPtr;
                    if (neighbor == null || neighbor->IsClosed)
                    {
                        continue;
                    }

                    scoreH = GetH(neighbor->Coordinate.X, neighbor->Coordinate.Y, to.X, to.Y);

                    var neighborWeight = GetCellWeightMultiplier(neighbor);
                    
                    var neighborTravelWeight = GetNeighborTravelWeightMultiplier(
                        current->Coordinate.X, current->Coordinate.Y,
                        neighbor->Coordinate.X, neighbor->Coordinate.Y);

                    var scoreG = current->ScoreG + (neighborTravelWeight * scoreH) + (neighborWeight * scoreH);

                    if (!_openSet.Contains(neighbor))
                    {
                        neighbor->ParentCoordinate = current->Coordinate;
                        neighbor->Depth = current->Depth + 1;
                        neighbor->ScoreG = scoreG;
                        neighbor->ScoreH = scoreH;

                        var scoreF = scoreG + scoreH;

                        _openSet.Enqueue(neighbor, scoreF); //ScoreF set by the queue
                    }
                    else if (scoreG + neighbor->ScoreH < neighbor->ScoreF)
                    {
                        neighbor->ScoreG = scoreG;
                        neighbor->ScoreF = scoreG + neighbor->ScoreH;
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
                last = _cellProvider.GetCellPointer(last->ParentCoordinate.X, last->ParentCoordinate.Y);
            }

            result.Path = stack;

            return result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetNeighborTravelWeightMultiplier(int startX, int startY, int destX, int destY)
        {
            return IsDiagonalMovement(startX, startY, destX, destY)
                ? _settings.DiagonalMovementMultiplier
                : _settings.StraightMovementMultiplier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetCellWeightMultiplier(Cell* cell)
        {
            return _settings.IsCellWeightEnabled ? cell->Weight : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PopulateNeighbors(Cell* current, int agentSize, ref Cell*[] neighbors)
        {
            var position = current->Coordinate;

            PopulateNeighbor(position.X - 1, position.Y, agentSize, ref neighbors, West);
            PopulateNeighbor(position.X + 1, position.Y, agentSize, ref neighbors, East);
            PopulateNeighbor(position.X, position.Y + 1, agentSize, ref neighbors, South);
            PopulateNeighbor(position.X, position.Y - 1, agentSize, ref neighbors, North);

            if (!_settings.IsDiagonalMovementEnabled)
            {
                for (var i = DiagonalStart; i < neighbors.Length; i++)
                {
                    neighbors[i] = null;
                }

                return;
            }

            var canGoWest = neighbors[West] != null;
            var canGoEast = neighbors[East] != null;
            var canGoSouth = neighbors[South] != null;
            var canGoNorth = neighbors[North] != null;
            var isCornersCutAllowed = _settings.IsMovementBetweenCornersEnabled;

            PopulateNeighbor(position.X - 1, position.Y + 1, agentSize, ref neighbors, SouthWest,
                canGoWest || canGoSouth || isCornersCutAllowed);
            PopulateNeighbor(position.X - 1, position.Y - 1, agentSize, ref neighbors, NorthWest,
                canGoWest || canGoNorth || isCornersCutAllowed);
            PopulateNeighbor(position.X + 1, position.Y + 1, agentSize, ref neighbors, SouthEast,
                canGoEast || canGoSouth || isCornersCutAllowed);
            PopulateNeighbor(position.X + 1, position.Y - 1, agentSize, ref neighbors, NorthEast,
                canGoEast || canGoNorth || isCornersCutAllowed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PopulateNeighbor(int x, int y, int agentSize, ref Cell*[] neighbors,
            int neighborIndex, bool shouldPopulate = true)
        {
            if (!shouldPopulate)
            {
                neighbors[neighborIndex] = null;
                return;
            }

            neighbors[neighborIndex] = GetWalkableLocation(x, y, agentSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Cell* GetWalkableLocation(int x, int y)
        {
            var cell = _cellProvider.GetCellPointer(x, y);

            return (_settings.IsCalculatingOccupiedCells && cell->IsOccupied) || !cell->IsWalkable
                ? null
                : cell;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Cell* GetWalkableLocation(int x, int y, int agentSize)
        {
            if (!IsPositionValid(x, y))
                return null;

            var location = GetWalkableLocation(x, y);

            if (location == null)
            {
                return null;
            }

            if (agentSize == SingleCellAgentSize)
            {
                return location;
            }

            //Clearance calculation
            for (var nY = 0; nY < agentSize; nY++)
            {
                for (var nX = 0; nX < agentSize; nX++)
                {
                    if (!IsPositionValid(x + nX, y + nY))
                        return null;
                    
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
            return x >= 0 && x < _cellProvider.Width && y >= 0 && y < _cellProvider.Height;
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