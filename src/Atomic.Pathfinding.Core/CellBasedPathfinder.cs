using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Helpers;
using Atomic.Pathfinding.Core.Interfaces;
using Atomic.Pathfinding.Core.Internal;

namespace Atomic.Pathfinding.Core
{
    /// <summary>
    /// Finds path in a cell based environment. <br/>
    /// Ideal for situations with limited amount of cells
    /// </summary>
    public class CellBasedPathfinder
    {
        private const double MaxHScoreBetweenNeighbors = 2;
        private const int MinPreloadedGridsAmount = 1;
        private const double CostTolerance = 0.01;
        

        private readonly Queue<LocationGrid> _locationGrids = new Queue<LocationGrid>();

        private readonly IGrid _grid;
        private readonly PathfinderSettings _settings;

        public CellBasedPathfinder(IGrid grid, PathfinderSettings settings = null,
            int preloadedGridsAmount = MinPreloadedGridsAmount)
        {
            if (grid?.Matrix == null || grid.Matrix.Length == 0)
                throw new EmptyGridException();

            _settings = settings ?? new PathfinderSettings();
            _grid = grid;

            preloadedGridsAmount = preloadedGridsAmount < MinPreloadedGridsAmount
                ? MinPreloadedGridsAmount
                : preloadedGridsAmount;

            for (var i = 0; i < preloadedGridsAmount; i++)
            {
                CreateLocationGrid();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PathResult GetPath(IAgent agent, Coordinate from, Coordinate to, bool postCallbackToAgent = false)
        {
            var grid = GetLocationGrid();

            var result = GetPathResult(agent, grid, from, to);

            if (postCallbackToAgent)
            {
                agent.OnPathResult(result);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private LocationGrid GetLocationGrid()
        {
            if (_locationGrids.Count == 0)
            {
                CreateLocationGrid();
            }

            return _locationGrids.Dequeue();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateLocationGrid()
        {
            var grid = new LocationGrid(_grid, _settings);
            _locationGrids.Enqueue(grid);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private PathResult GetPathResult(IAgent agent, LocationGrid grid, Coordinate from, Coordinate to)
        {
            grid.Reset();

            var start = grid.GetLocation(from);
            
            var scoreF = GetScoreH(from, to);
            grid.OpenSet.Enqueue(start, scoreF);

            while (grid.OpenSet.Count > 0)
            {
                grid.Current = grid.OpenSet.Dequeue();
                
                if (grid.Current.Position == to)
                {
                    break;
                }

                grid.Current.IsClosed = true;

                var neighbors = grid.GetNeighbors(grid.Current.Position, agent.Size);

                foreach (var neighbor in neighbors)
                {
                    if(neighbor == null || neighbor.IsClosed)
                    {
                        continue;
                    }

                    var scoreG = grid.Current.ScoreG
                                 + GetNeighborTravelWeight(grid.Current.Position, neighbor.Position)
                                 + GetCellWeight(neighbor.Position);

                    var priority = scoreG;

                    if (!grid.OpenSet.Contains(neighbor))
                    {
                        neighbor.SetParent(grid.Current);
                        neighbor.SetScoreG(scoreG);
                        
                        var scoreH = GetScoreH(neighbor.Position, to);
                        neighbor.SetScoreH(scoreH);
                        
                        grid.OpenSet.Enqueue(neighbor, neighbor.ScoreG + neighbor.ScoreH);
                    }
                    else if (scoreG + neighbor.ScoreH < neighbor.ScoreF)
                    {
                        neighbor.SetScoreG(scoreG);
                        neighbor.SetScoreF(neighbor.ScoreG + neighbor.ScoreH);
                        neighbor.SetParent(grid.Current);
                    }
                }
            }

            var result = new PathResult();

            if (grid.Current.Position == to)
            {
                result.Path = ReconstructPath(grid.Current);
            }

            _locationGrids.Enqueue(grid);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Coordinate[] ReconstructPath(Location last)
        {
            if (last == null)
                return null;

            var stack = new Stack<Coordinate>();

            while (last != null)
            {
                stack.Push(last.Position);
                last = last.Parent;
            }

            return stack.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double GetCellWeight(Coordinate position)
        {
            return _settings.IsCellWeightEnabled ? _grid.Matrix[position.Y, position.X].Weight : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double GetNeighborTravelWeight(Coordinate start, Coordinate destination)
        {
            var scoreH = GetScoreH(start, destination);

            if (scoreH > MaxHScoreBetweenNeighbors)
                throw new Exception("Can travel only to neighbors");

            return Math.Abs(scoreH - _settings.StraightMovementCost) < CostTolerance
                ? _settings.StraightMovementCost
                : _settings.DiagonalMovementCost;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetScoreH(Coordinate start, Coordinate destination)
        {
            return Math.Abs(destination.X - start.X) + Math.Abs(destination.Y - start.Y);
        }
    }
}