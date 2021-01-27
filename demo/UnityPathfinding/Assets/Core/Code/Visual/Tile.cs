using System;
using System.Collections;
using System.Collections.Generic;
using Atomic.Pathfinding.Core.Helpers;
using Atomic.Pathfinding.Core.Interfaces;
using UnityEngine;

public class Tile : MonoBehaviour, IGridCell
{
    public bool IsWalkable
    {
        get => _isWalkable;
        private set => SetWalkable(value);
    }

    public bool IsOccupied => !_isWalkable;
    public double Weight => 0;

    public (int, int) Coordinates
    {
        get => (_x, _y);
        set
        {
            _x = value.X();
            _y = value.Y();
        }
    }

    [SerializeField] private int _x;
    [SerializeField] private int _y;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private bool _isWalkable = true;
    [SerializeField] private double _weight = 1;
    [SerializeField] private GameObject _wall;

    private void Awake()
    {
        IsWalkable = _isWalkable;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.cyan;
        var position = transform.position + (transform.localScale * 0.5f);
        position.y -= 0.5f;
        UnityEditor.Handles.Label(position, $"{_x}:{_y}");
    }
    #endif

    public void SetMaterial(Material material)
    {
        _renderer.material = material;
    }

    private void SetWalkable(bool value)
    {
        _isWalkable = value;
        _wall.SetActive(!_isWalkable);
    }
}
