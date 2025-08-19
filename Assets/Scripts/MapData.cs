using UnityEngine;

namespace DefaultNamespace
{
    public class MapData : MonoBehaviour
    {
        [SerializeField] private Cell[] _cells;
        [TextArea]
        [SerializeField] private string _mapContext;
        
        public Cell[] Cells => _cells;
    }
}