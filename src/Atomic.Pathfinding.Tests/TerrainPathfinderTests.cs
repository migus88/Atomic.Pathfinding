using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Atomic.Pathfinding.Core;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Interfaces;
using Atomic.Pathfinding.Tests.Implementations;
using NUnit.Framework;
using static Atomic.Pathfinding.Tests.Implementations.GridCell;

namespace Atomic.Pathfinding.Tests
{
    [TestFixture]
    public class TerrainPathfinderTests
    {
        [Test]
        public void SingleAgent_AgentSize_Success_Test()
        {
            var maze = new Maze("Maze/Conditions/001.png");
            
            var start = maze.Start;
            var destination = maze.Destination;
            var cells = maze.Cells;
            var agent = new Agent { Size = 2 };

            var aStar = new TerrainPathfinder(maze.Width, maze.Height);
            

            var result = aStar.GetPath(ref cells, agent, start, destination);
            maze.AddPath((Coordinate[])result.Path);

            if (!Directory.Exists("Results/"))
            {
                Directory.CreateDirectory("Results/");
            }
            
            maze.SaveImage("Results/001.png", 100);
        }
    }
}