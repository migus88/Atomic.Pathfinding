using System;
using Atomic.Pathfinding.Benchmark.Maze;
using Atomic.Pathfinding.Core;
using BenchmarkDotNet.Running;

namespace Atomic.Pathfinding.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<MazeBenchmark>();
        }
    }
}