using System.Collections.Generic;
using System.Collections.ObjectModel;
using Atomic.Pathfinding.Benchmark.Maze;
using BenchmarkDotNet.Attributes;

namespace Atomic.Pathfinding.Benchmark
{
    [MemoryDiagnoser(false)]
    public class MazeBenchmarkRunner
    {
        private readonly (int x, int y) _start = (10, 10);
        private readonly (int x, int y) _destination = (502, 374);
        
        private static readonly string AtomicRunner = nameof(AtomicMazeBenchmarkRunner);
        private static readonly string RoyTRunner = nameof(RoyTAStarMazeBunchmarkRunner);

        private readonly Dictionary<string, IMazeBenchmarkRunner> _benchmarkRunners =
            new Dictionary<string, IMazeBenchmarkRunner>
            {
                [AtomicRunner] = new AtomicMazeBenchmarkRunner(),
                [RoyTRunner] = new RoyTAStarMazeBunchmarkRunner(),
            };
        
        public MazeBenchmarkRunner()
        {
            foreach (var benchmarkRunner in _benchmarkRunners.Values)
            {
                benchmarkRunner.Init(null);
            }
        }

        public void PrintAllResults()
        {
            foreach (var benchmarkRunner in _benchmarkRunners.Values)
            {
                benchmarkRunner.RenderPath(_start, _destination);
            }
        }

        [Benchmark] public void AtomicPathfinding() => _benchmarkRunners[AtomicRunner].FindPathBenchmark(_start, _destination);
        [Benchmark] public void RoyTAStar() => _benchmarkRunners[RoyTRunner].FindPathBenchmark(_start, _destination);
    }
}