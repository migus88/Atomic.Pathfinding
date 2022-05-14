using Atomic.Pathfinding.Benchmark.Maze;
using BenchmarkDotNet.Attributes;

namespace Atomic.Pathfinding.Benchmark
{
    [MemoryDiagnoser(false)]
    public class MazeBenchmarkRunner
    {
        private readonly (int x, int y) _start = (10, 10);
        private readonly (int x, int y) _destination = (502, 374);
        
        private readonly IMazeBenchmarkRunner _atomicBenchmarkRunnerRunner;
        private readonly IMazeBenchmarkRunner _royTaStarBenchmarkRunner;
        
        public MazeBenchmarkRunner()
        {
            _atomicBenchmarkRunnerRunner = new AtomicMazeBenchmarkRunner();
            _atomicBenchmarkRunnerRunner.Init(null);
            
            _royTaStarBenchmarkRunner = new RoyTAStarMazeBunchmarkRunner();
            _royTaStarBenchmarkRunner.Init(null);
        }

        [Benchmark] public void AtomicPathfinding() => _atomicBenchmarkRunnerRunner.FindPathBenchmark(_start, _destination);
        [Benchmark] public void RoyTAStar() => _royTaStarBenchmarkRunner.FindPathBenchmark(_start, _destination);
    }
}