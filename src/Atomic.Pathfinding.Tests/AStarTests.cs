using System;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Tests.Implementations;
using NUnit.Framework;
using static Atomic.Pathfinding.Tests.Implementations.GridCell;

namespace Atomic.Pathfinding.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void SingleAgent_AgentSize_Success_Test()
        {
            var matrix = new GridCell[,] //Example from here: https://harablog.wordpress.com/2009/01/29/clearance-based-pathfinding/
            {
                { _, _, _, _, _, _, _, _, _, _ },
                { _, _, _, _, _, _, _, _, X, _ },
                { _, _, _, _, _, _, _, _, _, _ },
                { _, _, _, X, _, _, _, _, _, _ },
                { _, _, X, _, _, _, _, _, X, _ },
                { _, _, X, X, X, _, _, X, _, _ },
                { _, _, _, X, X, _, X, _, _, X },
                { _, _, _, _, X, _, _, _, _, X },
                { _, _, _, _, _, _, _, _, _, _ },
                { _, _, _, _, _, _, _, _, _, _ },
            };
            

            var grid = new Grid(matrix);
            var agent = new Agent
            {
                Size = 2,
                PathResultAction = result =>
                {
                    grid.UpdatePath(result.Path);
                    Console.WriteLine(grid);
                }
            };

            var aStar = new Pathfinding.Core.AStar(grid);
            aStar.AddAgent(agent);

            var start = (0, 8);
            var destination = (8, 2);

            var path = aStar.GetPath(agent, start, destination);
            
            Assert.IsTrue(path.IsPathFound);
            
            agent.OnPathResult(path);
        }
        
        [Test]
        public void SingleAgent_AgentSize_Fail_Test()
        {
            var matrix = new GridCell[,] //Example from here: https://harablog.wordpress.com/2009/01/29/clearance-based-pathfinding/
            {
                { _, _, _, _, _, _, _, _, _, _ },
                { _, _, _, _, _, _, _, _, X, _ },
                { _, _, _, _, _, _, _, _, _, _ },
                { _, _, _, X, _, _, _, _, _, _ },
                { _, _, X, _, _, _, _, _, X, _ },
                { _, _, X, X, X, _, _, X, _, _ },
                { _, _, _, X, X, _, X, _, _, X },
                { _, _, _, _, X, _, _, _, _, X },
                { _, _, _, _, _, _, _, _, _, _ },
                { _, _, _, _, _, _, _, _, _, _ },
            };
            

            var grid = new Grid(matrix);
            var agent = new Agent
            {
                Size = 3,
                PathResultAction = result =>
                {
                    grid.UpdatePath(result.Path);
                    Console.WriteLine(grid);
                }
            };

            var aStar = new Pathfinding.Core.AStar(grid);
            aStar.AddAgent(agent);

            var start = (0, 8);
            var destination = (8, 2);

            var path = aStar.GetPath(agent, start, destination);
            
            Assert.IsFalse(path.IsPathFound);

            Console.WriteLine("The path for agent with size 3 doesn't exist.");
        }


        [Test]
        public void SingleAgent_CornersCutDisabled_Test()
        {
            var matrix = new GridCell[,]
            {
                { _, _, _, X, _, _, _, _, _, _ },
                { _, _, _, X, _, _, _, _, _, _ },
                { _, _, X, _, _, _, _, _, _, _ },
                { _, X, X, _, _, _, _, _, _, _ },
                { _, _, _, _, _, _, _, _, _, _ },
                { _, _, _, _, _, _, _, _, _, _ },
                { _, _, _, _, _, _, _, _, _, _ },
                { _, _, _, _, _, _, _, _, _, _ },
                { _, _, _, _, _, _, _, _, _, _ },
                { _, _, _, _, _, _, _, _, _, _ },
            };


            var grid = new Grid(matrix);
            var agent = new Agent
            {
                Size = 1,
                PathResultAction = result =>
                {
                    grid.UpdatePath(result.Path);
                    Console.WriteLine(grid);
                }
            };

            var aStar = new Pathfinding.Core.AStar(grid);
            aStar.AddAgent(agent);

            var start = (0, 0);
            var destination = (3, 2);

            var path = aStar.GetPath(agent, start, destination);

            Assert.IsTrue(path.IsPathFound);

            agent.OnPathResult(path);

            Assert.IsFalse(((GridCell)grid.Matrix[1,2]).IsPath);
        }

        [Test]
        public void SingleAgent_CornersCutEnabled_Test()
        {
            var matrix = new GridCell[,]
            {
                { _, _, _, X, _, _, _, _, _, _ },
                { _, _, _, X, _, _, _, _, _, _ },
                { _, _, X, _, _, _, _, _, _, _ },
                { _, X, X, _, _, _, _, _, _, _ },
                { _, _, _, _, _, _, _, _, _, _ },
                { _, _, _, _, _, _, _, _, _, _ },
                { _, _, _, _, _, _, _, _, _, _ },
                { _, _, _, _, _, _, _, _, _, _ },
                { _, _, _, _, _, _, _, _, _, _ },
                { _, _, _, _, _, _, _, _, _, _ },
            };


            var grid = new Grid(matrix);
            var agent = new Agent
            {
                Size = 1,
                PathResultAction = result =>
                {
                    grid.UpdatePath(result.Path);
                    Console.WriteLine(grid);
                }
            };

            var settings = new PathfinderSettings
            {
                IsMovementBetweenCornersEnabled = true
            };

            var aStar = new Pathfinding.Core.AStar(grid, settings);
            aStar.AddAgent(agent);

            var start = (0, 0);
            var destination = (3, 2);

            var path = aStar.GetPath(agent, start, destination);

            Assert.IsTrue(path.IsPathFound);

            agent.OnPathResult(path);

            Assert.IsTrue(((GridCell)grid.Matrix[1, 2]).IsPath);
        }


        [Test]
        public void SingleAgent_WeightsEnabled_Test()
        {
            var matrix = new GridCell[,]
            {
                { _, a, a, a, a, _, _, _, _, _ },
                { _, _, _, _, _, _, _, _, _, _ }
            };


            var grid = new Grid(matrix);
            var agent = new Agent
            {
                Size = 1,
                PathResultAction = result =>
                {
                    grid.UpdatePath(result.Path);
                    Console.WriteLine(grid);
                }
            };

            var settings = new PathfinderSettings
            {
                IsMovementBetweenCornersEnabled = true,
                IsCellWeightEnabled = true
            };

            var aStar = new Pathfinding.Core.AStar(grid, settings);
            aStar.AddAgent(agent);

            var start = (0, 0);
            var destination = (5, 0);

            var path = aStar.GetPath(agent, start, destination);

            Assert.IsTrue(path.IsPathFound);

            agent.OnPathResult(path);

            Assert.IsFalse(((GridCell)grid.Matrix[0, 1]).IsPath);
        }


        [Test]
        public void SingleAgent_WeightsDisabled_Test()
        {
            var matrix = new GridCell[,]
            {
                { _, a, a, a, a, _, _, _, _, _ },
                { _, _, _, _, _, _, _, _, _, _ }
            };


            var grid = new Grid(matrix);
            var agent = new Agent
            {
                Size = 1,
                PathResultAction = result =>
                {
                    grid.UpdatePath(result.Path);
                    Console.WriteLine(grid);
                }
            };

            var settings = new PathfinderSettings
            {
                IsMovementBetweenCornersEnabled = true,
                IsCellWeightEnabled = false
            };

            var aStar = new Pathfinding.Core.AStar(grid, settings);
            aStar.AddAgent(agent);

            var start = (0, 0);
            var destination = (5, 0);

            var path = aStar.GetPath(agent, start, destination);

            Assert.IsTrue(path.IsPathFound);

            agent.OnPathResult(path);

            Assert.IsTrue(((GridCell)grid.Matrix[0, 1]).IsPath);
        }
    }
}