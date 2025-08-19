using System;
using DG.Tweening;
using UnityEngine;

public class ElementTransition : MonoBehaviour
{
    [SerializeField] private Transform movingTransform;
    [SerializeField] private Transform activePosition;
    [SerializeField] private Transform hidePosition;

    private void Start()
    {
        if (activePosition == null) activePosition = movingTransform;
        else if (hidePosition == null) hidePosition = movingTransform;
    }

#if UNITY_EDITOR
    [ContextMenu("ShowElement")]
    public void ShowElement()
    {
        ChangeActiveInstantly(true);
    }
    
    [ContextMenu("HideElement")]
    public void HideElement()
    {
        ChangeActiveInstantly(false);
    }
#endif
    
    public Tween ChangeActive(bool isActive)
    {
        return movingTransform.DOMove(isActive ? activePosition.position : hidePosition.position, 0.7f)
            .SetLink(gameObject)
            .SetEase(Ease.InOutExpo).Play();
    }
    
    public void ChangeActiveInstantly(bool isActive)
    {
        movingTransform.position = isActive ? activePosition.position : hidePosition.position;
    }
}