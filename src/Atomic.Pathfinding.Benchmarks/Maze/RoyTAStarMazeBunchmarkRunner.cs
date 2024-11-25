using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Tools;
using Roy_T.AStar.Graphs;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using Path = Roy_T.AStar.Paths.Path;

namespace Atomic.Pathfinding.Benchmarks.Maze;

public class RoyTAStarMazeBunchmarkRunner : BaseMazeBenchmarkRunner
{
    protected override string ResultImageName => nameof(RoyTAStarMazeBunchmarkRunner);
        
    private readonly PathFinder _pathFinder = new PathFinder();
    private Node[,] _nodes;
    private Grid _grid;
        
    public override void Init(Maze<Cell> maze)
    {
        base.Init(maze);
            

        _nodes = new Node[_maze.Width, _maze.Height];
        PopulateNodes();

        _grid = Grid.CreateGridFrom2DArrayOfNodes(_nodes);
    }

    public override void FindPathBenchmark((int x, int y) start, (int x, int y) destination)
    {
        var path = GetPath(start, destination);
    }

    public override void RenderPath((int x, int y) start, (int x, int y) destination)
    {
        var result = GetPath(start, destination);
        var coordinates = new Coordinate[result.Edges.Count + 1];
        var firstCoordinate = result.Edges[0].End.Position;
        coordinates[0] = new Coordinate((int)firstCoordinate.X, (int)firstCoordinate.Y);

        for (var i = 0; i < result.Edges.Count; i++)
        {
            var coordinate = result.Edges[i].End.Position;
            coordinates[i + 1] = new Coordinate((int)coordinate.X, (int)coordinate.Y);
        }
            
        _maze.AddPath(coordinates);
            
        SaveMazeResultAsImage();
    }

    private Path GetPath((int x, int y) start, (int x, int y) destination)
    {
        var startPoint = new GridPosition(start.x, start.y);
        var endPoint = new GridPosition(destination.x, destination.y);
            
        var path = _pathFinder.FindPath(startPoint, endPoint, _grid);

        if (path.Edges.Count == 0)
        {
            throw new Exception("Path not found");
        }

        return path;
    }
        
        

    private void PopulateNodes()
    {
        var cells = _maze.Cells;
            
        for (var y = 0; y < _maze.Height; y++)
        {
            for (var x = 0; x < _maze.Width; x++)
            {
                _nodes[x, y] = new Node(new Position(x, y));
            }
        }
            
        for (var y = 0; y < _maze.Height; y++)
        {
            for (var x = 0; x < _maze.Width; x++)
            {
                Connect(x, y, cells);
            }
        }
    }

    private void Connect(int x, int y, Cell[,] cells)
    {
        var node = _nodes[x, y];

        var neighbors = new (int, int)[]
        {
            (x + 1, y),
            (x - 1, y),
            (x, y + 1),
            (x, y - 1),
            (x + 1, y + 1),
            (x - 1, y - 1),
            (x + 1, y - 1),
            (x - 1, y + 1)
        };

        var velocity = Velocity.FromMetersPerSecond(1);

        foreach (var neighbor in neighbors)
        {
            if (neighbor.Item1 < 0 || neighbor.Item1 >= _maze.Width || neighbor.Item2 < 0 ||
                neighbor.Item2 >= _maze.Height)
            {
                continue;
            }

            var cell = cells[neighbor.Item1, neighbor.Item2];
            if (cell.IsOccupied || !cell.IsWalkable)
            {
                continue;
            }
                
            node.Connect(_nodes[neighbor.Item1, neighbor.Item2], velocity);
        }
    }
}