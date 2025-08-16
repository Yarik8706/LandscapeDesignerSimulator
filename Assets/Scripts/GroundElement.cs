using UnityEngine;

namespace DefaultNamespace
{
    public class GroundElement : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        public GroundElementData GroundElementData { get; private set; }
        
        public void SetData(GroundElementData groundElementData)
        {
            this.GroundElementData = groundElementData;
            _spriteRenderer.sprite = groundElementData.icon;
        }
    }
}