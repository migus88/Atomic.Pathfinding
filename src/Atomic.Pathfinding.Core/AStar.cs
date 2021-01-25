using System;
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

        private readonly Dictionary<IAgent, LocationGrid> _agentGrids = new Dictionary<IAgent, LocationGrid>();
        private readonly Dictionary<IAgent, bool> _agentBusiness = new Dictionary<IAgent, bool>();
        private readonly Queue<LocationGrid> _unusedGrids = new Queue<LocationGrid>();

        private readonly IGrid _grid;
        private readonly PathfinderSettings _settings;

        public AStar(IGrid grid, PathfinderSettings settings = null)
        {
            if (grid == null || grid.Matrix == null || grid.Matrix.Length == 0)
                throw new EmptyGridException();

            _settings = settings ?? new PathfinderSettings();
            _grid = grid;
        }

        public void AddAgent(IAgent agent)
        {
            if (_agentGrids.HasKey(agent))
                throw new AgentAlreadyExistsException();

            if(agent.Size <= 0)
                throw new AgentTooSmallException();

            var height = _grid.Matrix.GetLength(0);
            var width = _grid.Matrix.GetLength(1);
            
            if(agent.Size > height || agent.Size > width)
                throw new AgentTooBigException();
            
            LocationGrid grid;

            if (_unusedGrids.Count > 0)
            {
                grid = _unusedGrids.Dequeue();
            }
            else
            {
                grid = new LocationGrid(_grid, _settings);
            }

            _agentGrids.Add(agent, grid);
            _agentBusiness.Add(agent, false);
        }

        public void RemoveAgent(IAgent agent)
        {
            if (_agentGrids.TryGetValue(agent, out var grid))
            {
                _unusedGrids.Enqueue(grid);
                _agentGrids.Remove(agent);
                _agentBusiness.Remove(agent);
            }
        }

        public bool IsAgentBusy(IAgent agent) => _agentBusiness.GetValueOrDefault(agent, false);

        public async void GetPathInNewThread(IAgent agent, (int, int) from, (int, int) to)
        {
            ValidateAgent(agent, out var grid);

            _agentBusiness[agent] = true;

            PathResult? result = null;

            ThreadPool.QueueUserWorkItem(w =>
            {
                result = GetPathResult(agent, grid, from, to);
            });

            while (result == null)
            {
                await Task.Yield();
            }

            _agentBusiness[agent] = false;

            agent.OnPathResult(result.Value);
        }

        public async Task<PathResult> GetPathAsync(IAgent agent, (int, int) from, (int, int) to)
        {
            ValidateAgent(agent, out var grid);

            _agentBusiness[agent] = true;
            var result = await Task.Run(() => GetPathResult(agent, grid, from, to));
            _agentBusiness[agent] = false;

            return result;
        }

        public PathResult GetPath(IAgent agent, (int, int) from, (int, int) to)
        {
            ValidateAgent(agent, out var grid);

            _agentBusiness[agent] = true;
            var result = GetPathResult(agent, grid, from, to);
            _agentBusiness[agent] = false;

            return result;
        }

        private void ValidateAgent(IAgent agent, out LocationGrid grid)
        {
            if (_agentGrids.TryGetValue(agent, out var value))
            {
                if (_agentBusiness[agent])
                {
                    throw new AgentBusyException();
                }

                grid = value;
            }
            else
            {
                throw new AgentNotFoundException();
            }
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

            return scoreH == HScorePerStraightMovement ? _settings.StraightMovementCost : _settings.DiagonalMovementCost;
        }

        private int GetScoreH((int, int) start, (int, int) destination)
        {
            return Math.Abs(destination.X() - start.X()) + Math.Abs(destination.Y() - start.Y());
        }
    }
}
