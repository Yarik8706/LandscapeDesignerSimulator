using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class CellContextMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject _menu;

        [SerializeField] private TMP_Text _cellInfo;
        
        [SerializeField] private Image[] _icons;
        [SerializeField] private TMP_Text[] _labels;
        
        public static CellContextMenuUI Instance { get; private set; }
        private void Awake() => Instance = this;

        public void Show(Cell cell)
        {
            _menu.SetActive(true);
            _icons[0].sprite = cell.data.ground.icon;
            _labels[0].text = cell.data.ground.name;
            for (int i = 1; i-1 < cell.data.decorations.Length; i++)
            {
                if (cell.data.decorations[i] == null)
                {
                    _icons[i].gameObject.SetActive(false);
                    _labels[i].gameObject.SetActive(false);
                    continue;
                }
                _icons[i].sprite = cell.data.decorations[i]?.icon;
                _labels[i].text = cell.data.decorations[i]?.name;
            }
        }
    }
}