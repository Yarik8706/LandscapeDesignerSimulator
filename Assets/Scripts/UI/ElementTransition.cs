using System;
using DG.Tweening;
using UnityEngine;

public class ElementTransition : MonoBehaviour
{
    [SerializeField] private Transform movingTransform;
    [SerializeField] private Transform activePosition;
    [SerializeField] private Transform hidePosition;
    [SerializeField] private float duration = 0.7f;

    private void Start()
    {
        if (activePosition == null) activePosition = movingTransform;
        else if (hidePosition == null) hidePosition = movingTransform;
    }

#if UNITY_EDITOR
    [ContextMenu("ShowElement")]
    public void ShowElementInstantly()
    {
        ChangeActiveInstantly(true);
    }
    
    [ContextMenu("HideElement")]
    public void HideElementInstantly()
    {
        ChangeActiveInstantly(false);
    }
#endif
    
    public void ShowElement() => ChangeActive(true);
    public void HideElement() => ChangeActive(false);
    
    public Tween ChangeActive(bool isActive)
    {
        return movingTransform.DOMove(isActive ? activePosition.position : hidePosition.position, duration)
            .SetLink(gameObject)
            .SetEase(Ease.InOutExpo).Play();
    }
    
    public void ChangeActiveInstantly(bool isActive)
    {
        movingTransform.position = isActive ? activePosition.position : hidePosition.position;
    }
}

public static class ElementTransitionExtensions
{
    public static Tween ChangeActive(this Transform movingTransform, Vector3 newPosition, float duration = 0.7f)
    {
        return movingTransform.DOMove(newPosition, duration)
            .SetLink(movingTransform.gameObject)
            .SetEase(Ease.InOutExpo).Play();
    }
}