using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Atomic.Pathfinding.Core;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Interfaces;
using Atomic.Pathfinding.Tests.Implementations;
using Atomic.Pathfinding.Tools;
using NUnit.Framework;

namespace Atomic.Pathfinding.Tests
{
    [TestFixture]
    public class TerrainPathfinderTests
    {
        [Test]
        public void SingleAgent_AgentSize_Success_Test()
        {
            var maze = new Maze<Cell>("Maze/Conditions/001.png");
            
            var start = maze.Start;
            var destination = maze.Destination;
            var agent = new Agent { Size = 2 };

            var aStar = new Pathfinder<Cell>(maze.Width, maze.Height);
            

            var result = aStar.GetPath(maze, agent, start, destination);
            maze.AddPath((Coordinate[])result.Path);

            if (!Directory.Exists("Results/"))
            {
                Directory.CreateDirectory("Results/");
            }
            
            maze.SaveImage("Results/001.png", 100);
        }
        
        [Test]
        public void Cavern_Test()
        {
            var maze = new Maze<Cell>("Maze/Conditions/000.gif");
            var start = new Coordinate(10, 10);
            var destination = new Coordinate(502, 374);
            
            maze.SetStart(start);
            maze.SetDestination(destination);
            var agent = new Agent { Size = 1 };

            var aStar = new Pathfinder<Cell>(maze.Width, maze.Height);
            
            //Jitting for more accurate stopwatch result
            aStar.GetPath(maze, agent, start, destination);

            var sw = new Stopwatch();
            sw.Start();
            var result = aStar.GetPath(maze, agent, start, destination);
            sw.Stop();

            Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds} ms");

            var closedCount = 0;
            foreach (var cell in maze.Cells)
            {
                if (cell.IsClosed)
                {
                    closedCount++;
                    maze.SetClosed(cell.Coordinate);
                }
            }

            Console.WriteLine($"Closed count: {closedCount}");
            
            if(result.IsPathFound)
            {
                maze.AddPath((Coordinate[]) result.Path);
            }

            if (!Directory.Exists("Results/"))
            {
                Directory.CreateDirectory("Results/");
            }
            
            maze.SaveImage("Results/000.png", 4);
            
            Assert.IsTrue(result.IsPathFound);
        }
    }
}