using UnityEngine;

namespace DefaultNamespace
{
    public class BuildElement : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        public BuildElementData BuildElementData { get; set; }
        
        public void SetData(BuildElementData buildElementData, int layerIndex = -1)
        {
            this.BuildElementData = buildElementData;
            _spriteRenderer.sortingOrder = layerIndex;
        }
    }
}