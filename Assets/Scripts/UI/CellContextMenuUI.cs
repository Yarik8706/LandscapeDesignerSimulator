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
            _icons[0].sprite = cell.data.ground.icon;
            _labels[0].text = cell.data.ground.displayName;
            if (cell.data.decorations[0] == null && cell.data.decorations[1] != null)
            {
                _icons[2].gameObject.SetActive(false);
                _labels[2].gameObject.SetActive(false);
                _icons[1].sprite = cell.data.decorations[1]?.icon;
                _labels[1].text = cell.data.decorations[1]?.displayName;
                return;
            }
            for (int i = 1; i-1 < 2; i++)
            {
                if (cell.data.decorations[i-1] == null)
                {
                    _icons[i].gameObject.SetActive(false);
                    _labels[i].gameObject.SetActive(false);
                    continue;
                }
                _icons[i].sprite = cell.data.decorations[i-1]?.icon;
                _labels[i].text = cell.data.decorations[i-1]?.displayName;
            }
        }
    }
}