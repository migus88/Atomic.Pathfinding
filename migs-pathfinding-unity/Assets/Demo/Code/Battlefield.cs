using System;
using System.Collections;
using Migs.Pathfinding.Core;
using Migs.Pathfinding.Core.Data;
using Migs.Pathfinding.Core.Interfaces;
using Code.Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Demo
{
    public class Battlefield : MonoBehaviour, ICellProvider
    {
        // In a real project, please use a DI container to manage the dependencies and not this...
        // I'm using a singleton here in order to not add 3rd party dependencies to the project
        // just for the sake of this demo
        public static Battlefield Instance { get; private set; }
        
        public Pathfinder Pathfinder { get; private set; }
        
        public event Action<Cell> CellClicked;

        [SerializeField] private ScriptablePathfinderSettings _settings;
        [SerializeField] private Player _player;
        [SerializeField] private Vector2Int _fieldSize;
        [SerializeField] private FieldCell[] _fieldCells;
        
        private void Awake()
        {
            // 🤮
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Pathfinder = new Pathfinder(this, _settings);
            Array.Sort(_fieldCells, Utils.FieldCellComparison);
            
            foreach (var fieldCell in _fieldCells)
            {
                fieldCell.CellClicked += OnCellClicked;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Randomize Weight")]
        public void RandomizeWeight()
        {
            foreach (var cell in _fieldCells)
            {
                cell.SetWeight(Random.Range(1f,10f));
            }
        }

        [ContextMenu("Visualize Weights")]
        public void VisualizeWeights()
        {
            foreach (var cell in _fieldCells)
            {
                cell.VisualizeWeight();
            }
        }
#endif

        private int GetFieldIndex(int column, int row)
        {
            var index = row * _fieldSize.x + column;
                
            if (index >= _fieldCells.Length || row < 0 || column < 0)
            {
                throw new IndexOutOfRangeException();
            }
                
            return index;
        }        
        
        public FieldCell GetFieldCell(int x, int y)
        {
            var index = GetFieldIndex(x, y);
            return _fieldCells[index];
        }
        
        
        // Basically here's the magic happens. This will allow the pathfinder to do its thing.
        // Everything else is just a demo setup.
        #region ICellProvider implementation
        
        public int Width => _fieldSize.x;
        public int Height => _fieldSize.y;
        
        public unsafe Cell* GetCellPointer(int x, int y)
        {
            var index = GetFieldIndex(x, y);
            fixed (Cell* cellPtr = &_fieldCells[index].Cell)
            {
                return cellPtr;
            }
        }

        public void ResetCells()
        {
            foreach (var fieldCell in _fieldCells)
            {
                fieldCell.Cell.Reset();
            }
        }


        #endregion

        private void OnCellClicked(FieldCell clickedCell)
        {
            CellClicked?.Invoke(clickedCell.Cell);
        }  

        private void OnDestroy()
        {
            if (_fieldCells == null)
            {
                return;
            }
            
            foreach (var fieldCell in _fieldCells)
            {
                if(fieldCell)
                {
                    fieldCell.CellClicked -= OnCellClicked;
                }
            }
        }
    }
}