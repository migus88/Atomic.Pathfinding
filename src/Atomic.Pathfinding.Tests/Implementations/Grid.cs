using System;
using System.Collections.Generic;
using System.Text;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Helpers;
using Atomic.Pathfinding.Core.Interfaces;

namespace Atomic.Pathfinding.Tests.Implementations
{
    public class Grid : IGrid, IEquatable<Grid>
    {
        public IGridCell[,] Matrix { get; private set; }

        public Grid(GridCell[,] matrix)
        {
            Matrix = matrix;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("\r\n|");

            for (int i = 0; i < Matrix.GetLength(1); i++)
            {
                builder.Append("===");
            }
            
            builder.Append("|\r\n|");

            for (int y = 0; y < Matrix.GetLength(0); y++)
            {
                for (int x = 0; x < Matrix.GetLength(1); x++)
                {
                    builder.Append(Matrix[y,x].ToString());
                }
                builder.Append("|\r\n|");
            }

            builder.Remove(builder.Length - 2, 2);
            
            builder.Append("|");

            for (int i = 0; i < Matrix.GetLength(1); i++)
            {
                builder.Append("===");
            }
            
            builder.Append("|");

            return builder.ToString();
        }

        public void UpdatePath(Coordinate[] path)
        {
            var matrix = Matrix as GridCell[,];

            foreach (var item in matrix)
            {
                item.IsPath = false;
            }

            foreach (var item in path)
            {
                matrix[item.Y, item.X].IsPath = true;
            }
        }

        public bool Equals(Grid other)
        {
            if (Matrix.Length != other.Matrix.Length)
                return false;

            for (int y = 0; y < Matrix.GetLength(0); y++)
            {
                for (int x = 0; x < Matrix.GetLength(1); x++)
                {
                    if (!Matrix[y, x].Equals(other.Matrix[y, x]))
                        return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if(obj is Grid other)
            {
                return this.Equals(other);
            }

            return base.Equals(obj);
        }
    }

    public class GridCell : IGridCell, IEquatable<GridCell>
    {
        public bool IsWalkable { get; set; }
        public bool IsOccupied { get; set; }
        public bool IsPath { get; set; }
        public double Weight { get; set; }

        public static GridCell _ => new GridCell { IsWalkable = true, IsOccupied = false, Weight = 0 };
        public static GridCell a => new GridCell { IsWalkable = true, IsOccupied = false, Weight = 1 };
        public static GridCell O => new GridCell { IsWalkable = true, IsOccupied = true };
        public static GridCell X => new GridCell { IsWalkable = false, IsOccupied = false };
        public static GridCell P => new GridCell { IsWalkable = true, IsOccupied = false, IsPath = true };


        public override string ToString()
        {
            if (IsPath)
                return " ◎ ";


            if(IsOccupied)
            {
                return " ▽ ";
            }    
            else if (IsWalkable)
            {
                return " ◻ ";
            }
            else //(!IsWalkable)
            {
                return " ◼ ";
            }
        }

        public bool Equals(GridCell other)
        {
            return IsWalkable == other.IsWalkable && IsOccupied == other.IsOccupied && IsPath == other.IsPath;
        }

        public override bool Equals(object obj)
        {
            if (obj is GridCell other)
                return this.Equals(other);

            return base.Equals(obj);
        }
    }
}
