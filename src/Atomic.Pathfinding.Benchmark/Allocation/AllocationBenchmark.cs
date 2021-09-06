using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Atomic.Pathfinding.Benchmark.Allocation
{
    [MemoryDiagnoser]
    public class AllocationBenchmark
    {
        private const int Columns = 100;
        private const int Rows = 100;

        [Benchmark]
        public void ManyStructsInStruct()
        {
            var amount = Columns * Rows;

            var holder = new StructStructsHolder(amount);
        }
        

        [Benchmark]
        public void ManyStructsInClass()
        {
            var amount = Columns * Rows;

            var holder = new ClassStructsHolder(amount);
        }

        [Benchmark]
        public void TestStructHolder()
        {
            var matrix = new StructHolder[Columns, Rows];

            for (short x = 0; x < Columns; x++)
            {
                for (short y = 0; y < Rows; y++)
                {
                    matrix[x, y] = new StructHolder(new BasicStruct(x, y));
                }
            }

            foreach (var item in matrix)
            {
                var retrieved = item;
                var x = item.BasicStruct.X;
                var y = item.BasicStruct.Y;
            }
        }

        [Benchmark]
        public void TestStructsOnly()
        {
            var matrix = new BasicStruct[Columns, Rows];

            for (short x = 0; x < Columns; x++)
            {
                for (short y = 0; y < Rows; y++)
                {
                    matrix[x, y] = new BasicStruct(x, y);
                }
            }

            foreach (var item in matrix)
            {
                var retrieved = item;
                var x = item.X;
                var y = item.Y;
            }
        }

        [Benchmark]
        public void TestValueHolder()
        {
            var matrix = new ValueHolder[Columns, Rows];

            for (short x = 0; x < Columns; x++)
            {
                for (short y = 0; y < Rows; y++)
                {
                    matrix[x, y] = new ValueHolder(x, y);
                }
            }

            foreach (var item in matrix)
            {
                var retrieved = item;
                var x = item.X;
                var y = item.Y;
            }
        }

        private struct StructStructsHolder
        {
            public List<BasicStruct> Structs { get; set; }

            public StructStructsHolder(int amount)
            {
                Structs = new List<BasicStruct>(amount);
                
                for (int i = 0; i < amount; i++)
                {
                    var s = new BasicStruct(1, 1);
                    Structs.Add(s);
                }
            }
        }

        private class ClassStructsHolder
        {
            public List<BasicStruct> Structs { get; set; }

            public ClassStructsHolder(int amount)
            {
                Structs = new List<BasicStruct>(amount);
                
                for (int i = 0; i < amount; i++)
                {
                    var s = new BasicStruct(1, 1);
                    Structs.Add(s);
                }
            }
        }

        private class ValueHolder
        {
            public short X { get; }
            public short Y { get; }

            public ValueHolder(short x, short y)
            {
                X = x;
                Y = y;
            }
        }

        private class StructHolder
        {
            public BasicStruct BasicStruct { get; }

            public StructHolder(BasicStruct basicStruct)
            {
                BasicStruct = basicStruct;
            }
        }

        private struct BasicStruct
        {
            public short X { get; }
            public short Y { get; }

            public BasicStruct(short x, short y)
            {
                X = x;
                Y = y;
            }
        }
    }
}