using System;
using Atomic.Pathfinding.Core.Data;
using UnityEngine;

namespace Demo
{
    public class FieldCell : MonoBehaviour
    {
        public Cell Cell;
        
        public event Action<FieldCell> CellClicked;

        [SerializeField] private bool _isWalkable;
        [SerializeField] private Vector2Int _position;

        private void Awake()
        {
            Cell = new Cell
            {
                Coordinate = new Coordinate(_position.x, _position.y),
                IsWalkable = _isWalkable
            };
        }
        
        private void OnMouseDown() => CellClicked?.Invoke(this);
    }
}