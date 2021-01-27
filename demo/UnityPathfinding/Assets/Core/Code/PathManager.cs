using System;
using System.Collections;
using System.Collections.Generic;
using Atomic.Pathfinding.Core;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance; //TODO: remove this bullshit
    
    [SerializeField] private Grid3D _grid;
    [SerializeField] private Agent _agent;
    [SerializeField] private int _destX;
    [SerializeField] private int _destY;
    

    private AStar _pathfinder;
    private bool _isNavigating = false;

    private void Awake()
    {
        Instance = this; //TODO: remove this bullshit
    }

    private void Start()
    {
        _pathfinder = new AStar(_grid);
    }

    public void OnTileClicked(int x, int y)
    {
        if (_isNavigating)
        {
            return;
        }
        
        _destX = x;
        _destY = y;
        FindPath();
    }

    public void OnReachedDestination()
    {
        _isNavigating = false;
    }

    [ContextMenu("Find Path")]
    public void FindPath()
    {
        _isNavigating = true;

        var startCoordinates = _agent.CurrentPosition;
        
        _pathfinder.GetPathInNewThread(_agent, startCoordinates, (_destX, _destY));
    }
}
