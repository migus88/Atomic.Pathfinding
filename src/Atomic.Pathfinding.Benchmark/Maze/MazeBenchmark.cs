using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Atomic.Pathfinding.Benchmark.CellBased;
using Atomic.Pathfinding.Core;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Helpers;
using Atomic.Pathfinding.Core.Interfaces;
using Atomic.Pathfinding.Core.Internal;
using BenchmarkDotNet.Attributes;

namespace Atomic.Pathfinding.Benchmark.Maze
{
    [MemoryDiagnoser]
    public class MazeBenchmark
    {
        private TerrainPathfinder _pathfinder;
        private short _width;
        private short _height;
        private IAgent _agent;
        private IEnumerable<Coordinate> _path;
        private Coordinate _start;
        private Coordinate _destination;
        private Cell[] _cells;


        public MazeBenchmark()
        {
            var image = (Bitmap) Image.FromFile("cavern.gif");
            _width = (short)image.Width;
            _height = (short)image.Height;
            _agent = new Agent();

            _cells = new Cell[_height * _width];
            
            for (short y = 0; y < _height; y++)
            {
                for (short x = 0; x < _width; x++)
                {
                    var isWalkable = image.GetPixel(x, y).ToArgb() == Color.White.ToArgb();
                    var index = Utils.GetCellIndex(x, y, _width);
                    
                    _cells[index].SetIsWalkable(isWalkable);
                    _cells[index].SetCoordinate(new Coordinate(x, y));
                    _cells[index].InitQueueItem(index);
                }
            }
            
            _pathfinder = new TerrainPathfinder(_width, _height);
        }

        [Benchmark]
        public void Find()
        {
            _start = new Coordinate(10,10);
            _destination =  new Coordinate((short)(_width - 10),(short)(_height - 10));
            var result = _pathfinder.GetPath(_cells, _agent, _start, _destination);
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
                            if (_cells[Utils.GetCellIndex(x, y, _width)].IsOccupied)
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