using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Helpers;
using Atomic.Pathfinding.Core.Interfaces;
using Atomic.Pathfinding.Core.Internal;
using static Atomic.Pathfinding.Core.Helpers.DirectionIndexes;

namespace Atomic.Pathfinding.Core
{
    public sealed unsafe class Pathfinder<T> where T : unmanaged, ICell
    {
        private const int MaxNeighbors = 8;
        private const int SingleCellAgentSize = 1;

        private readonly PathfinderSettings _settings;
        private readonly int _width;
        private readonly int _height;

        private readonly FastPriorityQueue<T> _openSet;

        public Pathfinder(int width, int height, PathfinderSettings settings = null)
        {
            _settings = settings ?? new PathfinderSettings();

            _width = width;
            _height = height;
            _openSet = new FastPriorityQueue<T>(_width * _height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PathResult GetPath(ICellProvider<T> provider, IAgent agent, Coordinate from, Coordinate to)
        {
            if (!IsPositionValid(to.X, to.Y))
                throw new Exception("Destination is not valid");

            provider.ResetCells();
            _openSet.Clear();

            var scoreH = GetH(from.X, from.Y, to.X, to.Y);

            var current = provider.GetCellPointer(from.X, from.Y);
            _openSet.Enqueue(current, scoreH); //ScoreF set by the queue

            var neighbors = new T*[MaxNeighbors];

            while (_openSet.Count > 0)
            {
                current = _openSet.Dequeue();

                if (current->Coordinate == to)
                {
                    break;
                }

                current->IsClosed = true;

                PopulateNeighbors(ref provider, current, agent.Size, ref neighbors);

                foreach (var neighbor in neighbors)
                {
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
                last = provider.GetCellPointer(last->ParentCoordinate.X, last->ParentCoordinate.Y);
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
        private float GetCellWeightMultiplier(T* cell)
        {
            return _settings.IsCellWeightEnabled ? cell->Weight : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PopulateNeighbors(ref ICellProvider<T> provider, T* current, int agentSize, ref T*[] neighbors)
        {
            var position = current->Coordinate;

            PopulateNeighbor(ref provider, position.X - 1, position.Y, agentSize, ref neighbors, West);
            PopulateNeighbor(ref provider, position.X + 1, position.Y, agentSize, ref neighbors, East);
            PopulateNeighbor(ref provider, position.X, position.Y + 1, agentSize, ref neighbors, South);
            PopulateNeighbor(ref provider, position.X, position.Y - 1, agentSize, ref neighbors, North);

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

            PopulateNeighbor(ref provider, position.X - 1, position.Y + 1, agentSize, ref neighbors, SouthWest,
                canGoWest || canGoSouth || isCornersCutAllowed);
            PopulateNeighbor(ref provider, position.X - 1, position.Y - 1, agentSize, ref neighbors, NorthWest,
                canGoWest || canGoNorth || isCornersCutAllowed);
            PopulateNeighbor(ref provider, position.X + 1, position.Y + 1, agentSize, ref neighbors, SouthEast,
                canGoEast || canGoSouth || isCornersCutAllowed);
            PopulateNeighbor(ref provider, position.X + 1, position.Y - 1, agentSize, ref neighbors, NorthEast,
                canGoEast || canGoNorth || isCornersCutAllowed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PopulateNeighbor(ref ICellProvider<T> provider, int x, int y, int agentSize, ref T*[] neighbors,
            int neighborIndex, bool shouldPopulate = true)
        {
            if (!shouldPopulate)
            {
                neighbors[neighborIndex] = null;
                return;
            }

            neighbors[neighborIndex] = GetWalkableLocation(ref provider, x, y, agentSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T* GetWalkableLocation(ref ICellProvider<T> provider, int x, int y)
        {
            var cell = provider.GetCellPointer(x, y);

            return (_settings.IsCalculatingOccupiedCells && cell->IsOccupied) || !cell->IsWalkable
                ? null
                : cell;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T* GetWalkableLocation(ref ICellProvider<T> provider, int x, int y, int agentSize)
        {
            if (!IsPositionValid(x, y))
                return null;

            var location = GetWalkableLocation(ref provider, x, y);

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