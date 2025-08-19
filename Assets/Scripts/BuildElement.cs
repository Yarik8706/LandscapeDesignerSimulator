using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class BuildElement : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public BuildElementData BuildElementData { get; set; }
        
        public void SetData(BuildElementData buildElementData, int layerIndex = -1)
        {
            BuildElementData = buildElementData;
            _spriteRenderer.sortingOrder += layerIndex;
        }
    }
}