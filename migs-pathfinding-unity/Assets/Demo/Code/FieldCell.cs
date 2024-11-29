using System;
using Migs.Pathfinding.Core.Data;
using UnityEngine;

namespace Demo
{
    public class FieldCell : MonoBehaviour
    {
        // Notice that this is a field and not a property in order to grab its pointer.
        public Cell Cell;
        
        public event Action<FieldCell> CellClicked;

        [SerializeField] private bool _isWalkable;
        [SerializeField] private Vector2Int _position;
        [SerializeField, Range(1,10)] private float _weight = 1;
        
        [Header("Display")]
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _color = Color.white;
        [SerializeField] private Color _highestColor = Color.red;

        private void Awake()
        {
            Cell = new Cell
            {
                Coordinate = new Coordinate(_position.x, _position.y),
                IsWalkable = _isWalkable,
                Weight = _weight
            };
            
            VisualizeWeight();
        }

        [ContextMenu("Visualize Weight")]
        public void VisualizeWeight()
        {
            var position = transform.position;
            position = new Vector3(position.x, Mathf.Max(_weight - 1, 0) / 10, position.z);
            transform.position = position;
            
            _renderer.sharedMaterial.color = Color.Lerp(_color, _highestColor, _weight / 10);
        }

#if UNITY_EDITOR
        public void SetWeight(float weight)
        {
            _weight = weight;

            // Mark the script instance as dirty so changes persist
            UnityEditor.EditorUtility.SetDirty(this);

            // If you're using a prefab, also save the modifications
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);

            VisualizeWeight();
        }
#endif
        
        private void OnMouseDown() => CellClicked?.Invoke(this);
    }
}