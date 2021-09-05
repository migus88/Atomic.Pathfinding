using System;
using Atomic.Pathfinding.Benchmark.CellBased;
using BenchmarkDotNet.Running;

namespace Atomic.Pathfinding.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<CellBasedPathfinderBenchmarks>();
        }
    }
}