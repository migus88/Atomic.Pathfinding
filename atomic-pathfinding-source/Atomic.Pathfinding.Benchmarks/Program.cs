// See https://aka.ms/new-console-templat
using Atomic.Pathfinding.Benchmarks;using Atomic.Pathfinding.Benchmarks.MazeBenchmarks;
using BenchmarkDotNet.Running;

var benchmarkRunner = new MazeBenchmarkRunner();
//benchmarkRunner.PrintAllResults();
            
BenchmarkRunner.Run<MazeBenchmarkRunner>();