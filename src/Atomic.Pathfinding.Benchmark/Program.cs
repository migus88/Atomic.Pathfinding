using System;
using Atomic.Pathfinding.Benchmark.Allocation;
using Atomic.Pathfinding.Benchmark.CellBased;
using Atomic.Pathfinding.Benchmark.Maze;
using Atomic.Pathfinding.Benchmark.Terrain;
using Atomic.Pathfinding.Core;
using BenchmarkDotNet.Running;

namespace Atomic.Pathfinding.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var b = new MazeBenchmark();
            b.Find();
            b.RenderPath();
            // BenchmarkRunner.Run<MazeBenchmark>();
            // BenchmarkRunner.Run<CellBasedPathfinderBenchmarks>();
            // BenchmarkRunner.Run<AllocationBenchmark>();
            // BenchmarkRunner.Run<TerrainBenchmarker>();

            // new TerrainPathfinder(100);
        }
    }
}