using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class CellContextMenuUI : MonoBehaviour
    {
        [SerializeField] private ElementTransition _menu;

        [SerializeField] private TMP_Text _cellInfo;
        
        [SerializeField] private Image[] _icons;
        [SerializeField] private TMP_Text[] _labels;
        
        public static CellContextMenuUI Instance { get; private set; }
        private void Awake() => Instance = this;

        public void Show(Cell cell)
        {
            _menu.ChangeActive(true);
            _cellInfo.text = 
                "Твердость почвы: " + (cell.SoilStrength == SoilStrength.Strong 
                    ? "<color=green>сильная</color>" 
                    : "<color=red>слабая</color>") + "\n" + 
                "Пригодна для растений: " + (cell.DirtType == DirtType.Bad 
                    ? "<color=red>непригодна</color>" 
                    : "<color=green>пригодна</color>");
            SetupCellInfo(cell.data.ground.icon, cell.data.ground.displayName, 0);
            if (cell.data.decorations[0] == null && cell.data.decorations[1] != null)
            {
                _icons[2].gameObject.SetActive(false);
                _labels[2].gameObject.SetActive(false);
                SetupCellInfo(cell.data.decorations[1].icon, cell.data.decorations[1].displayName, 1);
                return;
            }
            for (int i = 1; i-1 < 2; i++)
            {
                Debug.Log("dsgdsfs");
                if (cell.data.decorations[i-1] == null)
                {
                    _icons[i].gameObject.SetActive(false);
                    _labels[i].gameObject.SetActive(false);
                    continue;
                }
                _icons[i].gameObject.SetActive(true);
                _labels[i].gameObject.SetActive(true);
                SetupCellInfo(cell.data.decorations[i-1].icon, cell.data.decorations[i-1].displayName, i);
            }
        }

        public void SetupCellInfo(Sprite sprite, string name, int index)
        {
            Debug.Log("dsdssdf");
            _icons[index].sprite = sprite;
            // set width and height of image how in icon
            _icons[index].rectTransform.sizeDelta = sprite.rect.size*6;

            _labels[index].text = name;
        }
    }
}