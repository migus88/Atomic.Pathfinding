using System.Drawing;
using System.Drawing.Imaging;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Helpers;

namespace Atomic.Pathfinding.Tools
{
    public class Maze
    {
        public int Width { get; }
        public int Height { get; }
        public Coordinate Start { get; private set; }
        public Coordinate Destination { get; private set; }
        public Cell[] Cells => _cells;
        
        private Bitmap _bitmap;
        private Cell[] _cells;

        public Maze(string path, bool createCells = true)
        {
            var bitmap = (Bitmap)Image.FromFile(path);
            Width = bitmap.Width;
            Height = bitmap.Height;

            _bitmap = new Bitmap(Width, Height);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _bitmap.SetPixel(x, y, bitmap.GetPixel(x, y));
                }
            }
            
            if(createCells)
            {
                CreateCells();
            }
        }

        public Maze(Cell[] cells, int width, int height, Coordinate start = default, Coordinate destination = default)
        {
            _cells = cells;
            Width = width;
            Height = height;
            Start = start;
            Destination = destination;
            
            CreateBitmap();
        }

        private void CreateBitmap()
        {
            _bitmap = new Bitmap(Width, Height);
            
            for (short x = 0; x < Width; x++)
            {
                for (short y = 0; y < Height; y++)
                {
                    var index = Utils.GetCellIndex(x, y, Width);
                    var cell = _cells[index];

                    var pixel = Color.White;

                    if (!cell.IsWalkable || cell.IsOccupied)
                    {
                        pixel = Color.Black;
                    }

                    var coordinates = new Coordinate(x, y);

                    if (Start == coordinates)
                    {
                        pixel = Color.Red;
                    }
                    else if (Destination == coordinates)
                    {
                        pixel = Color.Blue;
                    }
                    
                    _bitmap.SetPixel(x, y, pixel);
                }
            }
        }

        public void SetStart(Coordinate coordinate)
        {
            Start = coordinate;
            _bitmap.SetPixel(coordinate.X, coordinate.Y, Color.Red);
        }

        public void SetDestination(Coordinate coordinate)
        {
            Start = coordinate;
            _bitmap.SetPixel(coordinate.X, coordinate.Y, Color.Blue);
        }

        public void SetClosed(Coordinate coordinate)
        {
            _bitmap.SetPixel(coordinate.X, coordinate.Y, Color.Gray);
        }
        
        public void AddPath(Coordinate[] coordinates)
        {
            foreach (var coordinate in coordinates)
            {
                if (coordinate == Start || coordinate == Destination)
                {
                    continue;
                }

                _bitmap.SetPixel(coordinate.X, coordinate.Y, Color.Green);
            }
        }

        public void SaveImage(string path, int sizeMultiplier = 1)
        {
            if(sizeMultiplier == 1)
            {
                _bitmap.Save(path, ImageFormat.Png);
                return;
            }

            var bitmap = new Bitmap(Width * sizeMultiplier, Height * sizeMultiplier);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var color = _bitmap.GetPixel(x, y);
                    
                    for (int mX = 0; mX < sizeMultiplier; mX++)
                    {
                        for (int mY = 0; mY < sizeMultiplier; mY++)
                        {
                            bitmap.SetPixel(x * sizeMultiplier + mX, y * sizeMultiplier + mY, color);
                        }
                    }
                }
            }
            
            bitmap.Save(path, ImageFormat.Png);
        }

        public void CreateCells()
        {
            _cells = new Cell[Width * Height];
            for (short y = 0; y < Height; y++)
            {
                for (short x = 0; x < Width; x++)
                {
                    var index = Utils.GetCellIndex(x, y, Width);
                    var pixel = _bitmap.GetPixel(x, y);

                    ref var cell = ref _cells[index];
                    cell.IsWalkable = IsWalkable(pixel);
                    cell.Coordinate = new Coordinate(x, y);

                    if (IsStart(pixel))
                    {
                        Start = new Coordinate(x, y);
                    }
                    else if (IsDestination(pixel))
                    {
                        Destination = new Coordinate(x, y);
                    }
                }
            }
        }

        public bool IsWalkable(int x, int y) => IsWalkable(_bitmap.GetPixel(x, y));
        public bool IsBlocked(int x, int y) => IsBlocked(_bitmap.GetPixel(x, y));
        public bool IsStart(int x, int y) => IsStart(_bitmap.GetPixel(x, y));
        public bool IsDestination(int x, int y) => IsDestination(_bitmap.GetPixel(x, y));
        public bool IsPath(int x, int y) => IsPath(_bitmap.GetPixel(x, y));

        private bool IsWalkable(Color pixel) => !IsBlocked(pixel);
        private bool IsBlocked(Color pixel) => pixel.ToArgb() == Color.Black.ToArgb();
        private bool IsStart(Color pixel) => pixel.ToArgb() == Color.Red.ToArgb();
        private bool IsDestination(Color pixel) => pixel.ToArgb() == Color.Blue.ToArgb();
        private bool IsPath(Color pixel) => pixel.ToArgb() == Color.Green.ToArgb();
    }
}