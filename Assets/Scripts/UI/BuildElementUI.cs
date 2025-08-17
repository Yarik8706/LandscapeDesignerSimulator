using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildElementUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private Button _button;
    [SerializeField] private Button _showDetailsButton;
        
    private BuildElementData _buildElementData;

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
        _showDetailsButton.onClick.AddListener(ShowDetails);
    }

    public void SetData(BuildElementData buildElementData)
    {
        _name.text = buildElementData.displayName;
        _buildElementData = buildElementData;
        _icon.sprite = ObjectManager.Instance.objectContext.constructionObjects[buildElementData.id].sprite;
    }
        
    private void ShowDetails()
    {
        BuildElementDetailsUI.Instance.Show(_buildElementData);
    }

    private void OnClick()
    {
        if (_buildElementData == null)
            return;
        BuildSystem.Instance.SelectObject(_buildElementData);
    }
}