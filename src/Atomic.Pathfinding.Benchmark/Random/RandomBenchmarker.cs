using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Internal;
using BenchmarkDotNet.Attributes;

namespace Atomic.Pathfinding.Benchmark.Random
{
    [MemoryDiagnoser]
    public class RandomBenchmarker
    {
        [Benchmark]
        public void ArraySize()
        {
            var arr = new TestStruct[1];
            arr[0].F = 12;
        }
        
        [Benchmark]
        public void ClassSize()
        {
            var arr = new TestClass[] { new TestClass() };
            arr[0].F = 12;
        }
        
        [Benchmark]
        public void OptimizedArraySize()
        {
            var arr = new OptimizedStruct[1];
            arr[0].F = 12;
        }
        
        [Benchmark]
        public void OptimizedClassSize()
        {
            var arr = new OptimizedClass[] { new OptimizedClass() };
            arr[0].F = 12;
        }
    }

    public struct TestStruct
    {
        public bool IsClosed { get; set; }
        public Coordinate Coordinate { get; set; }
        public float F { get; set; }
        public float H { get; set; }
        public float G { get; set; }
        public int Depth { get; set; }
        
        public bool IsWalkable { get; set; }
        
        public bool IsOccupied { get; set; }
        
        public float Weight { get; set; }
        public Coordinate ParentCoordinate { get; set; }
        public bool IsInitialized { get; set; }
    }

    public class TestClass
    {
        public bool IsClosed { get; set; }
        public Coordinate Coordinate { get; set; }
        public float F { get; set; }
        public float H { get; set; }
        public float G { get; set; }
        public int Depth { get; set; }
        
        public bool IsWalkable { get; set; }
        
        public bool IsOccupied { get; set; }
        
        public float Weight { get; set; }
        public Coordinate ParentCoordinate { get; set; }
        public bool IsInitialized { get; set; }
    }

    public struct OptimizedStruct
    {
        public bool IsClosed { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public float F { get; set; }
        public float H { get; set; }
        public float G { get; set; }
        public int Depth { get; set; }
        public int QueueIndex { get; set; }
        public bool IsWalkable { get; set; }
        public bool IsOccupied { get; set; }
        public float Weight { get; set; }
        public short ParentX { get; set; }
        public short ParentY { get; set; }
    }

    public class OptimizedClass
    {
        public bool IsClosed { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public float F { get; set; }
        public float H { get; set; }
        public float G { get; set; }
        public int Depth { get; set; }
        public int QueueIndex { get; set; }
        public bool IsWalkable { get; set; }
        public bool IsOccupied { get; set; }
        public float Weight { get; set; }
        public OptimizedClass Parent { get; set; }
    }
}