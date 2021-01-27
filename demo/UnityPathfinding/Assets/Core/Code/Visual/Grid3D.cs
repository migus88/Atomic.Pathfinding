using System;
using System.Collections;
using System.Collections.Generic;
using Atomic.Pathfinding.Core.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

public class Grid3D : MonoBehaviour, IGrid
{
    public IGridCell[,] Matrix { get; private set; }
    
    [SerializeField] private Vector2 _cellSize;
    [SerializeField] private int _columnsCount;
    [SerializeField] private float _minY;
    [SerializeField] private float _maxY;
    [SerializeField] private Material[] _materials;
    [SerializeField] private Tile[] _tiles;
    

    private void Awake()
    {
        UpdateMatrix();
    }

    //I know it's code duplication, but for the simplicity of the example we'll leave it as is
    private void UpdateMatrix()
    {
        var rowsCount = (int)Math.Ceiling((double)_tiles.Length / _columnsCount);
        
        Matrix = new IGridCell[_columnsCount, rowsCount];
        
        var y = 0;
        var x = 0;
        
        foreach (var tile in _tiles)
        {
            Matrix[y, x] = tile;
            
            y++;

            if (y != _columnsCount) 
                continue;
            
            y = 0;
            x++;
        }
    }


#if UNITY_EDITOR
    
    [ContextMenu("Update Field Positions")]
    public void UpdateFieldPositions()
    {
        var currentCol = 0;
        var currentRow = 0;

        foreach (Tile tile in _tiles)
        {
            tile.transform.position = new Vector3(_cellSize.x * currentCol, Random.Range(_minY, _maxY),_cellSize.y * currentRow);
            tile.SetMaterial(_materials[Random.Range(0, _materials.Length)]);
            tile.Coordinates = (currentRow, currentCol);

            currentCol++;

            if (currentCol == _columnsCount)
            {
                currentCol = 0;
                currentRow++;
            }
            
            UnityEditor.EditorUtility.SetDirty(tile);
        }
        
    }
    
#endif
    
}
