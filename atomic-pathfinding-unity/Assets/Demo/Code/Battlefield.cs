using System;
using System.Collections;
using Atomic.Pathfinding.Core;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Interfaces;
using UnityEngine;

namespace Demo
{
    public class Battlefield : MonoBehaviour, ICellProvider
    {
        [SerializeField] private Player _player;
        [SerializeField] private Vector2Int _fieldSize;
        [SerializeField] private FieldCell[] _fieldCells;

        private Pathfinder _pathfinder;
        private bool _isMoving;
        
        private void Start()
        {
            _pathfinder = new Pathfinder(this);
            Array.Sort(_fieldCells, FieldCellComparison);
            
            foreach (var fieldCell in _fieldCells)
            {
                fieldCell.CellClicked += OnCellClicked;
            }
        }

        private void OnCellClicked(FieldCell clickedCell)
        {
            if (_isMoving)
            {
                return;
            }
            
            var destination = clickedCell.Cell.Coordinate;
            var pathResult = _pathfinder.GetPath(_player, _player.Coordinate, destination);
            
            if(pathResult.IsPathFound)
            {
                StartCoroutine(MoveToPoint(pathResult.Path));
            }
        }    
        
        private static int FieldCellComparison(FieldCell a, FieldCell b)
        {
            var result = a.Cell.Coordinate.Y.CompareTo(b.Cell.Coordinate.Y);
            return result == 0 ? a.Cell.Coordinate.X.CompareTo(b.Cell.Coordinate.X) : result;
        }
        
        private IEnumerator MoveToPoint(Coordinate[] path)
        {
            _isMoving = true;
            
            foreach (var coordinate in path)
            {
                var index = GetFieldIndex(coordinate.X, coordinate.Y);
                var cell = _fieldCells[index];
                var cellPosition = cell.transform.position;
                var waypoint = new Vector3(cellPosition.x, _player.transform.position.y, cellPosition.z);
                
                while (Vector3.Distance(_player.transform.position, waypoint) > 0.01f)
                {
                    _player.transform.position = Vector3.Lerp(_player.transform.position, waypoint, _player.Speed * Time.deltaTime);

                    yield return null;
                }

                _player.transform.position = waypoint;
                _player.Coordinate = cell.Cell.Coordinate;
            }
            
            _isMoving = false;
        }
        
        #region ICellProvider implementation

        public int Width => _fieldSize.x;
        public int Height => _fieldSize.y;
        
        public unsafe Cell* GetCellPointer(int x, int y)
        {
            var index = GetFieldIndex(x, y);
            fixed (Cell* cellPtr = &_fieldCells[index].Cell)
            {
                return cellPtr;
            }
        }

        public void ResetCells()
        {
            for (var x = 0; x < _fieldSize.x; x++)
            {
                for (var y = 0; y < _fieldSize.y; y++)
                {
                    var index = GetFieldIndex(x, y);
                    ref var cell = ref _fieldCells[index];
                    
                    if (cell == null)
                    {
                        continue;
                    }
                    cell.Cell.Reset();
                }
            }
        }


        #endregion

        private int GetFieldIndex(int column, int row)
        {
            var index = row * _fieldSize.x + column;
                
            if (index >= _fieldCells.Length || row < 0 || column < 0)
            {
                throw new IndexOutOfRangeException();
            }
                
            return index;
        }

        private void OnDestroy()
        {
            if (_fieldCells == null)
            {
                return;
            }
            
            foreach (var fieldCell in _fieldCells)
            {
                if(fieldCell)
                {
                    fieldCell.CellClicked -= OnCellClicked;
                }
            }
        }
    }
}