using System;
using System.Collections;
using System.Collections.Generic;
using Atomic.Pathfinding.Core.Data;
using Atomic.Pathfinding.Core.Helpers;
using Atomic.Pathfinding.Core.Interfaces;
using UnityEngine;

public class Agent : MonoBehaviour, IAgent
{
    public int Size => _size;

    [SerializeField] private int _size = 1;

    private Vector3[] _path = null;
    
    public void OnPathResult(PathResult result)
    {
        if (result.IsPathFound)
        {
            _path = TuplePathToVector3Path(result.Path);
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
            result[i] = new Vector3(path[i].Y() + 0.5f, 0.5f, path[i].X() + 0.5f);
        }

        return result;
    }
    
    #if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if(_path == null)
            return;

        for (var i = 0; i < _path.Length; i++)
        {
            if(i == _path.Length - 1)
                break;
            
            var location = _path[i];
            var nextLocation = _path[i + 1];
            
            Gizmos.DrawLine(location, nextLocation);
        }
        
    }

#endif
}
