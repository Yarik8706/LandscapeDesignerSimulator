using UnityEngine;

namespace DefaultNamespace
{
    public class CellContextMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject _menu;
        public static CellContextMenuUI Instance { get; private set; }
        private void Awake() => Instance = this;

        public void Show(Cell cell)
        {
            
        }
    }
}