using System;
using System.Collections;
using System.Collections.Generic;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Helpers;
using Atomic.Pathfinding.Core.Interfaces;
using UnityEngine;

public class Agent : MonoBehaviour, IAgent
{
    private const float DistanceTolerance = 0.01f;
    private const float SquareMagnitudeTolerance = DistanceTolerance * DistanceTolerance;
    
    public (int,int) CurrentPosition { get; private set; }
    public int Size => _size;

    [SerializeField] private int _size = 1;
    [SerializeField] private float _speed;

    private List<(int, int)> _coordinatesPath = null;
    private Vector3[] _path = null;
    private int _currentPathIndex = -1;

    private void Awake()
    {
        var position = transform.position;
        CurrentPosition = ((int) position.x, (int) position.z);
    }

    private void Update()
    {
        if(_path == null || _currentPathIndex < 0 || _currentPathIndex >= _path.Length)
        {
            PathManager.Instance.OnReachedDestination();
            _currentPathIndex = -1;
            return;
        }

        CurrentPosition = _coordinatesPath[_currentPathIndex];
        var destination = _path[_currentPathIndex];

        var direction = (destination - transform.position).normalized;

        transform.position = transform.position + direction * (_speed * Time.deltaTime);

        if ((transform.position - destination).sqrMagnitude <= SquareMagnitudeTolerance)
        {
            _currentPathIndex++;
        }
    }

    public void OnPathResult(PathResult result)
    {
        if (result.IsPathFound)
        {
            _path = TuplePathToVector3Path(result.Path);
            _coordinatesPath = result.Path;
            _currentPathIndex = 0;
        }
        else
        {
            Debug.Log("Path not found");
        }
    }

    private Vector3[] TuplePathToVector3Path(List<(int, int)> path)
    {
        var result = new Vector3[path.Count];

        for (int i = 0; i < path.Count; i++)
        {
            result[i] = new Vector3(path[i].Y(), transform.position.y, path[i].X());
        }

        return result;
    }
    
    #if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if(_path == null)
            return;

        var offset = new Vector3(0.5f, 0.5f, 0.5f);

        for (var i = 0; i < _path.Length; i++)
        {
            if(i == _path.Length - 1)
                break;
            
            var location = _path[i] + offset;
            var nextLocation = _path[i + 1] + offset;
            
            Gizmos.DrawLine(location, nextLocation);
        }
        
    }

#endif
}
