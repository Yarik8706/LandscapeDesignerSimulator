using UnityEngine;

namespace DefaultNamespace
{
    public class BuildElement : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        public BuildElementData BuildElementData { get; private set; }
        
        public void SetData(BuildElementData buildElementData, int layerIndex = -1)
        {
            this.BuildElementData = buildElementData;
            _spriteRenderer.sprite = buildElementData.icon;
            _spriteRenderer.sortingOrder = layerIndex;
        }
    }
}