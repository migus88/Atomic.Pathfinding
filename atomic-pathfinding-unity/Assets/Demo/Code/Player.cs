using System;
using System.Collections;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Helpers;
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
        
        // This is a hacky way to reduce allocations.
        // Instead of allocating a new collection, we're using pointers,
        // and in order to use asynchronous code with pointers, we cast it to IntPtr.
        private IntPtr _path;
        private int _pathLength;

        private void Start()
        {
            Battlefield.Instance.CellClicked += OnCellClicked;
        }

        private unsafe void OnCellClicked(Cell cell)
        {
            if (_isMoving)
            {
                return;
            }
            
            var destination = cell.Coordinate;
            var result = Battlefield.Instance.Pathfinder.GetPath(this, Coordinate, destination);
            
            if(result.IsPathFound)
            {
                _path = (IntPtr)result.Path.ToCoordinateArrayPointer();
                _pathLength = result.Path.Length;
                StartCoroutine(MoveToPoint());
            }
        }
        
        
        private IEnumerator MoveToPoint()
        {
            _isMoving = true;

            for (var i = 0; i < _pathLength; i++)
            {
                var coordinate = GetCoordinate(i);
                var cell = Battlefield.Instance.GetFieldCell(coordinate.X, coordinate.Y);
                var waypoint = cell.transform.position;

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
        
        private unsafe Coordinate GetCoordinate(int index) => ((Coordinate*)_path)![index];

        private void OnDestroy()
        {
            Battlefield.Instance.CellClicked -= OnCellClicked;
        }
    }
}