using System;
using System.Collections;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Interfaces;
using UnityEngine;

namespace Demo
{
    public class Player : MonoBehaviour, IAgent
    {
        public Coordinate Coordinate { get; set; }
        public int Size => _size;
        public float Speed => _speed;

        [SerializeField] private float _speed = 5;
        [SerializeField, Range(1,2)] private int _size = 1;
        
        private bool _isMoving;

        private void Start()
        {
            Battlefield.Instance.CellClicked += OnCellClicked;
        }

        private void OnCellClicked(Cell cell)
        {
            if (_isMoving)
            {
                return;
            }
            
            var destination = cell.Coordinate;
            var result = Battlefield.Instance.Pathfinder.GetPath(this, Coordinate, destination);
            
            if(result.IsPathFound)
            {
                StartCoroutine(MoveToPoint(result.Path));
            }
        }
        
        
        private IEnumerator MoveToPoint(Coordinate[] path)
        {
            _isMoving = true;
            
            foreach (var coordinate in path)
            {
                var cell = Battlefield.Instance.GetFieldCell(coordinate.X, coordinate.Y);
                var cellPosition = cell.transform.position;
                var waypoint = new Vector3(cellPosition.x, transform.position.y, cellPosition.z);
                
                while (Vector3.Distance(transform.position, waypoint) > 0.01f)
                {
                    transform.position = Vector3.Lerp(transform.position, waypoint, Speed * Time.deltaTime);

                    yield return null;
                }

                transform.position = waypoint;
                Coordinate = cell.Cell.Coordinate;
            }
            
            _isMoving = false;
        }

        private void OnDestroy()
        {
            Battlefield.Instance.CellClicked -= OnCellClicked;
        }
    }
}