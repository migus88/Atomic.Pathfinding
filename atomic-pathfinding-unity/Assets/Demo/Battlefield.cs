using System.Collections;
using Atomic.Pathfinding.Core;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Interfaces;
using Atomic.Pathfinding.Core.Internal;
using UnityEngine;

namespace Demo
{
    public class Battlefield : MonoBehaviour, ICellProvider<Cell>
    {
        [SerializeField] private Player _player;
        [SerializeField] private Vector2Int _fieldSize;
        [SerializeField] private FieldCell[] _fieldCells;

        private Pathfinder<Cell> _pathfinder;
        private FieldCell[,] _field; // We can skip it and use _fieldCells array with Linq, but this is more performant
        private bool _isMoving;
        
        private void Awake()
        {
            var pq = new FastPriorityQueue<Cell>(10);
            
            //_pathfinder = new Pathfinder<Cell>(_fieldSize.x, _fieldSize.y);
            //_field = new FieldCell[_fieldSize.x, _fieldSize.y];
            
            //foreach (var fieldCell in _fieldCells)
            //{
            //    fieldCell.CellClicked += OnCellClicked;
            //    _field[fieldCell.Cell.Coordinate.X, fieldCell.Cell.Coordinate.Y] = fieldCell;
            //}
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
                var waypoint = _field[coordinate.X, coordinate.Y].transform.position;
                
                while (Vector3.Distance(transform.position, waypoint) > 0.01f)
                {
                    transform.position = Vector3.Lerp(transform.position, waypoint, _player.Speed * Time.deltaTime);

                    yield return null;
                }

                transform.position = waypoint;
            }
            
            _isMoving = false;
        }

        
        #region ICellProvider<Cell> implementation
        
        public unsafe Cell* GetCellPointer(int x, int y)
        {
            fixed (Cell* cellPtr = &_field[x, y].Cell)
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
                    ref var cell = ref _field[x, y];
                    cell.Cell.Reset();
                }
            }
        }

        #endregion
    }
}