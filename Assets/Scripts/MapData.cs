using UnityEngine;

namespace DefaultNamespace
{
    public class MapData : MonoBehaviour
    {
        [SerializeField] private Cell[] _cells;
        [TextArea]
        [SerializeField] private string _mapContext;
        
        [field: SerializeField] public Transform MapCenter { get; private set; }
        
        public Cell[] Cells => _cells;
    }
}