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
            // var b = new MazeBenchmark();
            //
            // for (int i = 0; i < 100; i++)
            // {
            //     b.Find();
            // }
            // Console.WriteLine($"Fail Count: {b.FailCount}\r\nTotal: {b.TotalCount}\r\nSuccess Count: {b.SuccessCount}");
            
            // b.Find();
            // b.RenderPath();
            BenchmarkRunner.Run<MazeBenchmark>();
            // Console.WriteLine($"Fail Count: {MazeBenchmark.FailCount}\r\nTotal: {MazeBenchmark.TotalCount}");
            // BenchmarkRunner.Run<CellBasedPathfinderBenchmarks>();
            // BenchmarkRunner.Run<AllocationBenchmark>();
            // BenchmarkRunner.Run<TerrainBenchmarker>();

            // new TerrainPathfinder(100);
        }
    }
}