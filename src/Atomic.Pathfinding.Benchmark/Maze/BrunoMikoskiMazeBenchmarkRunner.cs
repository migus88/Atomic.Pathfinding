using System;
using System.Collections.Generic;
using System.IO;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Tools;
using BrunoMikoski.Pahtfinding;
using BrunoMikoski.Pahtfinding.Grid;
using UnityEngine;

namespace Atomic.Pathfinding.Benchmark.Maze
{
    public class BrunoMikoskiMazeBenchmarkRunner : BaseMazeBenchmarkRunner
    {
        protected override string ResultImageName => nameof(BrunoMikoskiMazeBenchmarkRunner);
        
        private GridController _gridController;

        public override void Init(Maze<Cell> maze)
        {
            base.Init(maze);
            _gridController = new GridController(_maze.Width, _maze.Height);
            _gridController.GenerateTiles();
            PopulateGrid();
            Pathfinder.Initialize(_gridController);
        }

        public override void FindPathBenchmark((int x, int y) start, (int x, int y) destination)
        {
            var result = GetPath(start, destination);
        }

        public override void RenderPath((int x, int y) start, (int x, int y) destination)
        {
            var result = GetPath(start, destination);

            var coordinates = new Coordinate[result.Count];

            for (var i = 0; i < result.Count; i++)
            {
                coordinates[i] = new Coordinate(result[i].PositionX, result[i].PositionY);
            }
            
            _maze.AddPath(coordinates);
            
            SaveMazeResultAsImage();
        }

        private List<Tile> GetPath((int x, int y) start, (int x, int y) destination)
        {
            var startVector = new Vector2Int(start.x, start.y);
            var endVector = new Vector2Int(destination.x, destination.y);

            var result = Pathfinder.GetPath(startVector, endVector);

            if (result == null || result.Count == 0)
            {
                throw new Exception("Path not found");
            }

            return result;
        }

        private void PopulateGrid()
        {
            for (var y = 0; y < _maze.Height; y++)
            {
                for (var x = 0; x < _maze.Width; x++)
                {
                    var tileIndex = _gridController.TilePosToIndex(x, y);
                    var mazeTile = _maze.Cells[x, y];

                    var tileType = TileType.ROAD;
                    if (mazeTile.IsOccupied || !mazeTile.IsWalkable)
                    {
                        tileType = TileType.BLOCK;
                    }
                    
                    _gridController.SetTileType(tileIndex, tileType);
                }
            }
        }
    }
}