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
    /// <summary>
    /// Finds path in a cell based environment. <br/>
    /// Ideal for situations with limited amount of cells
    /// </summary>
    public class CellBasedPathfinder
    {
        private const double MaxHScoreBetweenNeighbors = 2;
        private const double HScorePerStraightMovement = 1;
        private const int MinPreloadedGridsAmount = 1;

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

        public async Task<PathResult> GetPathAsync(IAgent agent, Coordinate from, Coordinate to,
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

        private LocationGrid GetLocationGrid(int retryCount = 0)
        {
            if (_locationGrids.Count == 0)
            {
                CreateLocationGrid();
            }

            return _locationGrids.Dequeue();
        }

        private void CreateLocationGrid()
        {
            var grid = new LocationGrid(_grid, _settings);
            _locationGrids.Enqueue(grid);
        }

        private PathResult GetPathResult(IAgent agent, LocationGrid grid, Coordinate from, Coordinate to)
        {
            grid.Reset();

            var start = grid.GetLocation(from);
            
            var scoreF = GetScoreH(from, to);
            start.SetScoreF(scoreF);

            grid.OpenSet.Add(start);

            while (grid.OpenSet.Count > 0)
            {
                grid.Current = grid.OpenSet.GetLowestCostLocation();
                
                if (grid.Current.Position == to)
                {
                    break;
                }

                grid.ClosedSet.Add(grid.Current);
                grid.OpenSet.Remove(grid.Current);

                var neighbors = grid.GetNeighbors(grid.Current.Position, agent.Size);

                foreach (var neighbor in neighbors)
                {
                    if(neighbor == null)
                        continue;
                    
                    if (grid.ClosedSet.HasKey(neighbor))
                    {
                        continue;
                    }

                    var gScore = grid.Current.ScoreG
                                 + GetNeighborTravelWeight(grid.Current.Position, neighbor.Position)
                                 + GetCellWeight(neighbor.Position);

                    var isBestScore = false;

                    if (!grid.OpenSet.HasKey(neighbor))
                    {
                        isBestScore = true;
                        var scoreH = GetScoreH(neighbor.Position, to);
                        neighbor.SetScoreH(scoreH);
                        grid.OpenSet.Add(neighbor);
                    }
                    else if (gScore < neighbor.ScoreG)
                    {
                        isBestScore = true;
                    }

                    if (isBestScore)
                    {
                        neighbor.SetParent(grid.Current);
                        neighbor.SetScoreG(gScore);
                        neighbor.SetScoreF(neighbor.ScoreG + neighbor.ScoreH);
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

        private List<Coordinate> ReconstructPath(Location last)
        {
            if (last == null)
                return null;

            var list = new List<Coordinate>();
            Location current = last;

            while (current != null)
            {
                list.Insert(0, current.Position);
                current = current.Parent;
            }

            return list;
        }

        private double GetCellWeight(Coordinate position)
        {
            return _settings.IsCellWeightEnabled ? _grid.Matrix[position.Y, position.X].Weight : 0;
        }

        private double GetNeighborTravelWeight(Coordinate start, Coordinate destination)
        {
            var scoreH = GetScoreH(start, destination);

            if (scoreH > MaxHScoreBetweenNeighbors)
                throw new Exception("Can travel only to neighbors");

            return Math.Abs(scoreH - HScorePerStraightMovement) < 0.01
                ? _settings.StraightMovementCost
                : _settings.DiagonalMovementCost;
        }

        private int GetScoreH(Coordinate start, Coordinate destination)
        {
            return Math.Abs(destination.X - start.X) + Math.Abs(destination.Y - start.Y);
        }
    }
}