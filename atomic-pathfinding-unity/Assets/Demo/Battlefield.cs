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

        private Pathfinder<Cell> _pathfinder;
        private FieldCell[,] _field; // We can skip it and use _fieldCells array with Linq, but this is more performant
        private bool _isMoving;
        
        private void Start()
        {
            _pathfinder = new Pathfinder<Cell>(_fieldSize.x, _fieldSize.y);
            _field = new FieldCell[_fieldSize.x, _fieldSize.y];
            
            foreach (var fieldCell in _fieldCells)
            {
                fieldCell.CellClicked += OnCellClicked;
                _field[fieldCell.Cell.Coordinate.X, fieldCell.Cell.Coordinate.Y] = fieldCell;
            }
        }

        private void OnCellClicked(FieldCell clickedCell)
        {
            if (_isMoving)
            {
                return;
            }
            
            var destination = clickedCell.Cell.Coordinate;
            var pathResult = _pathfinder.GetPath(this, _player, _player.Coordinate, destination);
            
            if(pathResult.IsPathFound)
            {
                StartCoroutine(MoveToPoint(pathResult.Path));
            }
        }
        
        private IEnumerator MoveToPoint(Coordinate[] path)
        {
            _isMoving = true;
            
            foreach (var coordinate in path)
            {
                var cell = _field[coordinate.X, coordinate.Y];
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
        
        #region ICellProvider<Cell> implementation
        
        public unsafe IntPtr GetCellPointer(int x, int y)
        {
            fixed (Cell* cellPtr = &_field[x, y].Cell)
            {
                return (IntPtr)cellPtr;
            }
        }

        public void ResetCells()
        {
            for (var x = 0; x < _fieldSize.x; x++)
            {
                for (var y = 0; y < _fieldSize.y; y++)
                {
                    ref var cell = ref _field[x, y];
                    
                    if (cell == null)
                    {
                        continue;
                    }
                    cell.Cell.Reset();
                }
            }
        }

        #endregion

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