using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BuildElementDetailsUI : MonoBehaviour
    {
        [SerializeField] private ElementTransition _detailsPanel;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _description;
        
        public static BuildElementDetailsUI Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void Show(BuildElementData buildElementData)
        {
            _detailsPanel.ChangeActive(true);
            _icon.sprite = buildElementData.icon;
            _icon.rectTransform.sizeDelta = buildElementData.icon.rect.size * 6;
            _name.text = buildElementData.displayName;
            _description.text = BuildElementData.GetDescription(buildElementData) 
                                + buildElementData.additionalDescription;
        }
        
        public void Hide()
        {
            _detailsPanel.ChangeActive(false);
        }
    }
}