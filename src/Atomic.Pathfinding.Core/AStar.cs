using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Helpers;
using Atomic.Pathfinding.Core.Interfaces;
using Atomic.Pathfinding.Core.Internal;

namespace Atomic.Pathfinding.Core
{
    public class AStar
    {
        private const double MaxHScoreBetweenNeighbors = 2;
        private const double HScorePerStraightMovement = 1;
        private const int MaxRetriesToGetGrid = 3;
        private const int MinPreloadedGridsAmount = 1;

        private readonly ConcurrentQueue<LocationGrid> _locationGrids = new ConcurrentQueue<LocationGrid>();

        private readonly IGrid _grid;
        private readonly PathfinderSettings _settings;

        public AStar(IGrid grid, PathfinderSettings settings = null, int preloadedGridsAmount = MinPreloadedGridsAmount)
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

        public async Task<PathResult> GetPathAsync(IAgent agent, (int, int) from, (int, int) to,
            bool postCallbackToAgent = false)
        {
            var grid = GetLocationGrid();

            var result = await Task.Run(() => GetPathResult(agent, grid, from, to));

            if (postCallbackToAgent)
            {
                agent.OnPathResult(result);
            }

            return result;
        }

        public PathResult GetPath(IAgent agent, (int, int) from, (int, int) to, bool postCallbackToAgent = false)
        {
            var grid = GetLocationGrid();

            var result = GetPathResult(agent, grid, from, to);

            if (postCallbackToAgent)
            {
                agent.OnPathResult(result);
            }

            return result;
        }

        private LocationGrid GetLocationGrid(int retryCount = 0)
        {
            if (_locationGrids.Count == 0)
            {
                CreateLocationGrid();
            }

            if (_locationGrids.TryDequeue(out var grid))
                return grid;

            if (retryCount <= MaxRetriesToGetGrid)
            {
                retryCount++;
                return GetLocationGrid(retryCount);
            }

            //This exception will probably never be thrown, but potentially this can
            //happen when multiple client threads access the 'GetPath' method while
            //there is only one grid left in the queue more times than defined in 'MaxRetriesToGetGrid' const.
            //Generally it is not a good idea to access this class from different threads. If multithreaded
            //pathfinding is required, it's better to use the 'GetPathInNewThread' method.
            throw new LocationGridUnreachableException();
        }

        private void CreateLocationGrid()
        {
            var grid = new LocationGrid(_grid, _settings);
            _locationGrids.Enqueue(grid);
        }

        private PathResult GetPathResult(IAgent agent, LocationGrid grid, (int, int) from, (int, int) to)
        {
            grid.Reset();

            var start = grid.GetLocation(from);
            start.ScoreF = GetScoreH(from, to);

            grid.OpenSet.Add(from, start);

            while (grid.OpenSet.Count > 0)
            {
                grid.Current = grid.OpenSet.GetLowestCostLocation();

                if (grid.Current.Position == to)
                {
                    break;
                }

                grid.ClosedSet.Add(grid.Current.Position, grid.Current);
                grid.OpenSet.Remove(grid.Current.Position);

                var neighbors = grid.GetNeighbors(grid.Current.Position, agent.Size);

                foreach (var neighbor in neighbors)
                {
                    if (grid.ClosedSet.HasKey(neighbor.Position))
                    {
                        continue;
                    }

                    var gScore = grid.Current.ScoreG
                                 + GetNeighborTravelWeight(grid.Current.Position, neighbor.Position)
                                 + GetCellWeight(neighbor.Position);

                    var isBestScore = false;

                    if (!grid.OpenSet.HasKey(neighbor.Position))
                    {
                        isBestScore = true;
                        neighbor.ScoreH = GetScoreH(neighbor.Position, to);
                        grid.OpenSet.Add(neighbor.Position, neighbor);
                    }
                    else if (gScore < neighbor.ScoreG)
                    {
                        isBestScore = true;
                    }

                    if (isBestScore)
                    {
                        neighbor.Parent = grid.Current;
                        neighbor.ScoreG = gScore;
                        neighbor.ScoreF = neighbor.ScoreG + neighbor.ScoreH;
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

        private List<(int, int)> ReconstructPath(Location last)
        {
            if (last == null)
                return null;

            var list = new List<(int, int)>();
            Location current = last;

            while (current != null)
            {
                list.Insert(0, current.Position);
                current = current.Parent;
            }

            return list;
        }

        private double GetCellWeight((int, int) position)
        {
            return _settings.IsCellWeightEnabled ? _grid.Matrix[position.Y(), position.X()].Weight : 0;
        }

        private double GetNeighborTravelWeight((int, int) start, (int, int) destination)
        {
            var scoreH = GetScoreH(start, destination);

            if (scoreH > MaxHScoreBetweenNeighbors)
                throw new Exception("Can travel only to neighbors");

            return Math.Abs(scoreH - HScorePerStraightMovement) < 0.01
                ? _settings.StraightMovementCost
                : _settings.DiagonalMovementCost;
        }

        private int GetScoreH((int, int) start, (int, int) destination)
        {
            return Math.Abs(destination.X() - start.X()) + Math.Abs(destination.Y() - start.Y());
        }
    }
}