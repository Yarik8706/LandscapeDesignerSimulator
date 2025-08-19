using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildElementUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _name;
        
    private BuildElementData _buildElementData;

    public void SetData(BuildElementData buildElementData)
    {
        _name.text = buildElementData.displayName;
        _buildElementData = buildElementData;
        _icon.sprite = buildElementData.icon;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(eventData.button);
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ShowDetails();
        }
        else
        {
            OnClick();
        }
    }
    
}