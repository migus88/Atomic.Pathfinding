using System;
using Atomic.Pathfinding.Benchmark.CellBased;
using Atomic.Pathfinding.Benchmark.Maze;
using BenchmarkDotNet.Running;

namespace Atomic.Pathfinding.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            // var b = new MazeBenchmark();
            // b.Find();
            
            // BenchmarkRunner.Run<MazeBenchmark>();
            BenchmarkRunner.Run<CellBasedPathfinderBenchmarks>();
        }
    }
}