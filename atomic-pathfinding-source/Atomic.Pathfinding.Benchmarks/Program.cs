// See https://aka.ms/new-console-templat
using Atomic.Pathfinding.Benchmarks;using Atomic.Pathfinding.Benchmarks.MazeBenchmarks;
using BenchmarkDotNet.Running;

if(args.Length > 0 && args[0] == "profile")
{
    var benchmarkRunner = new AtomicMazeBenchmarkRunner();
    benchmarkRunner.Init(null);

    for (int i = 0; i < 100; i++)
    {
        Console.WriteLine("Running iteration: " + i);
        
        benchmarkRunner.FindPath((10, 10), (502, 374));
    }
}
else if (args.Length > 0 && args[0] == "internal")
{
    BenchmarkRunner.Run<InternalBenchmarkRunner>();
}
else
{
    BenchmarkRunner.Run<MazeBenchmarkRunner>();
}