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
        
        
        public void OnPathResult(PathResult result)
        {
            // Do Nothing
        }
        
    }
}