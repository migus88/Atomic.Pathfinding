using System;
using System.Collections;
using System.Collections.Generic;
using Atomic.Pathfinding.Core;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [SerializeField] private Grid3D _grid;
    [SerializeField] private Agent _agent;
    [SerializeField] private int _destX;
    [SerializeField] private int _destY;
    

    private AStar _pathfinder;

    private void Start()
    {
        _pathfinder = new AStar(_grid);
    }

    [ContextMenu("Find Path")]
    public void FindPath()
    {
        var agentPosition = _agent.transform.position;
        var startCoordinates = ((int)agentPosition.x, (int)agentPosition.y);
        
        _pathfinder.GetPathInNewThread(_agent, startCoordinates, (_destX, _destY));
    }
}
