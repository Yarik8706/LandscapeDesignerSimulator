using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class Cell : MonoBehaviour, IPointerClickHandler
    {
        public CellData data = new CellData();

        private GroundElement _groundElement;
        private BuildElement[] _buildElements;

        [SerializeField] private SoilStrength[] _soilsStrength;
        [SerializeField] private TerrainType _terrainType;
        [SerializeField] private ObjectContext _objectContext;
        [SerializeField] private BuildElement _buildElementPrefab;
        [SerializeField] private GroundElement _groundElementPrefab;

        private void Start()
        {
            data.ground = _objectContext.groundElements[(int)_terrainType];
            data.ground.soils = _soilsStrength;
        }

        private void OnValidate()
        {
            data.x = (int)transform.position.x;
            data.y = (int)transform.position.y;
        }

        public void AddBuildElement(BuildElementData buildElementData)
        {
            var newElement = Instantiate(_buildElementPrefab, 
                transform.position, 
                Quaternion.identity, transform.parent);
            newElement.SetData(buildElementData);
            
            data.decorations[(int)buildElementData.category] = newElement.BuildElementData;
            _buildElements[(int)buildElementData.category] = newElement;
        }
        
        public void RemoveBuildElement(Category category)
        {
            var buildElementData = data.decorations[(int)category];
            if (buildElementData == null) return;
            Destroy(_buildElements[(int)category].gameObject);
            data.decorations[(int)category] = null;
            _buildElements[(int)category] = null;
        }
        
        public void AddGroundElement(GroundElementData groundElementData)
        {
            var newElement = Instantiate(_groundElementPrefab, 
                transform.position, 
                Quaternion.identity, transform.parent);
            newElement.SetData(groundElementData);
            data.ground = newElement.GroundElementData;
            _groundElement = newElement;
        }
        
        public void RemoveGroundElement()
        {
            var groundElementData = data.ground;
            if (groundElementData == null) return;
            Destroy(_groundElement.gameObject);
            data.ground = null;
            _groundElement = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            BuildSystem.Instance.ClickOnCell(this);
        }
    }
    
    [Serializable]
    public class CellData
    {
        public int x;
        public int y;
        public int id;
        public GroundElementData ground;
        public bool hasGround;
        public BuildElementData[] decorations;

        public CellData()
        {
            decorations = new BuildElementData[2];
        }
    }
}