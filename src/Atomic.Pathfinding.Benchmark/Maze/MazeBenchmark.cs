using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Atomic.Pathfinding.Benchmark.CellBased;
using Atomic.Pathfinding.Core;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Interfaces;
using BenchmarkDotNet.Attributes;

namespace Atomic.Pathfinding.Benchmark.Maze
{
    [MemoryDiagnoser]
    public class MazeBenchmark
    {
        private IGridCell[,] _matrix;
        private IGrid _grid;
        private CellBasedPathfinder _pathfinder;
        private int _width;
        private int _height;
        private IAgent _agent;
        private Coordinate[] _path;
        private Coordinate _start;
        private Coordinate _destination;


        public MazeBenchmark()
        {
            var image = (Bitmap) Image.FromFile("cavern.gif");
            _width = image.Width;
            _height = image.Height;
            _agent = new Agent();

            _matrix = new IGridCell[_height, _width];
            
            for (var x = 0; x < _height; x++)
            {
                for (var y = 0; y < _width; y++)
                {
                    var cell = new ClassCellBasedGrid.GridCell();
                    cell.IsOccupied = (image.GetPixel(y, x).R + image.GetPixel(y, x).G + image.GetPixel(y, x).B) / 3 < 128;
                    
                    _matrix[x, y] = cell;
                }
            }

            _grid = new ClassCellBasedGrid(_matrix);
            _pathfinder = new CellBasedPathfinder(_grid);
        }

        [Benchmark]
        public void Find()
        {
            _start = new Coordinate {X = 10, Y = 10};
            _destination =  new Coordinate {X = _width - 10, Y = _height - 10};
            var result = _pathfinder.GetPath(_agent, _start, _destination);
            _path = result.Path;
        }
        
        
        
        public void RenderPath()
        {
            var scalar = 10;

            var verdana = new FontFamily("Verdana");
            var statsFont = new Font(verdana, 36, FontStyle.Bold, GraphicsUnit.Pixel);

            using (var image = new Bitmap(_width * scalar, _height * scalar, PixelFormat.Format32bppArgb))
            {
                using (var graphics = Graphics.FromImage(image))
                {
                    graphics.Clear(Color.White);

                    int closedCount = 0;

                    for (var y = 0; y < _width; y++)
                    {
                        for (var x = 0; x < _height; x++)
                        {
                            if (_matrix[x, y].IsOccupied)
                            {
                                graphics.FillRectangle(new SolidBrush(Color.DarkGray), y * scalar - scalar / 2,
                                    x * scalar - scalar / 2, scalar, scalar);
                            }
                        }
                    }

                    graphics.DrawLines(new Pen(new SolidBrush(Color.LimeGreen), 8),
                        _path.Select(n => new PointF(n.X * scalar, n.Y * scalar)).ToArray());

                    CircleAtPoint(graphics, new PointF(_start.X * scalar, _start.Y * scalar), 5, Color.Red);
                    CircleAtPoint(graphics, new PointF(_destination.X * scalar, _destination.Y * scalar), 5, Color.Green);

                    image.Save(
                        $"{_start.X}_{_start.Y}-{_destination.X}_{_destination.Y}.png",
                        ImageFormat.Png);
                }
            }
        }

        private void CircleAtPoint(Graphics graphics, PointF center, float radius, Color color)
        {
            var shifted = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            graphics.FillEllipse(new SolidBrush(color), shifted);
        }
    }
}