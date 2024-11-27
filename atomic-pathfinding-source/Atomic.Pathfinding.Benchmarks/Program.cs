// See https://aka.ms/new-console-templat
using Atomic.Pathfinding.Benchmarks;using Atomic.Pathfinding.Benchmarks.MazeBenchmarks;
using BenchmarkDotNet.Running;

if(args.Length > 0 && args[0] == "profile")
{
    Console.WriteLine("Press any key to start profiling...");
    //Console.ReadKey();
    
    var benchmarkRunner = new AtomicMazeBenchmarkRunner();
    benchmarkRunner.Init(null);

    for (int i = 0; i < 100; i++)
    {
        Console.WriteLine("Running iteration: " + i);
        
        benchmarkRunner.FindPathBenchmark((10, 10), (502, 374));
    }

    Console.WriteLine("Finished profiling. Press any key to exit...");
    //Console.ReadKey();
}
else
{
    BenchmarkRunner.Run<MazeBenchmarkRunner>();
}