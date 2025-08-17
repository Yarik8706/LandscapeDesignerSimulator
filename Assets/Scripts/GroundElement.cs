using UnityEngine;

namespace DefaultNamespace
{
    public class GroundElement : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        [field: SerializeField] public GroundElementData GroundElementData { get; private set; }
        
        public void SetData(GroundElementData groundElementData, int layerIndex = -1)
        {
            this.GroundElementData = groundElementData;
            _spriteRenderer.sprite = groundElementData.icon;
            _spriteRenderer.sortingOrder = layerIndex;
        }
    }
}