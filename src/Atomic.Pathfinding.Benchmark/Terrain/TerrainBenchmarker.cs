using Atomic.Pathfinding.Core;
using BenchmarkDotNet.Attributes;

namespace Atomic.Pathfinding.Benchmark.Terrain
{
    [MemoryDiagnoser()]
    public class TerrainBenchmarker
    {
        private const int Width = 100;
        private const int Height = 100;
        
        [Benchmark]
        public void TestSpanAllocation()
        {
            
        }
    }
}