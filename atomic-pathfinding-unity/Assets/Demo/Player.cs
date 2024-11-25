using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Interfaces;
using UnityEngine;

namespace Demo
{
    public class Player : MonoBehaviour, IAgent
    {
        public Coordinate Coordinate { get; set; }
        public int Size => 1;
        public float Speed => _speed;

        [SerializeField] private float _speed = 5;
        
        public void OnPathResult(PathResult result)
        {
            // Do Nothing
        }
    }
}