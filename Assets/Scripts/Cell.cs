using System;
using UnityEditor;
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
            data.soils = _soilsStrength;
        }

        private void OnValidate()
        {
            data.x = (int)transform.position.x;
            data.y = (int)transform.position.y;
            AddGroundElement(_objectContext.groundElements[(int)_terrainType], true);
        }

        public void AddBuildElement(BuildElementData buildElementData)
        {
            var newElement = Instantiate(_buildElementPrefab, 
                transform.position, 
                Quaternion.identity, transform.parent);
            newElement.SetData(buildElementData, -(int)transform.position.y);
            
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
        
        public void AddGroundElement(GroundElementData groundElementData, bool isEditor = false)
        {
            if(_groundElement != null && _groundElement.GroundElementData.id == groundElementData.id) return;
            if (_groundElement != null)
            {
                if (!isEditor) Destroy(_groundElement.gameObject);
            }
            GroundElement newElement = null;
            if (!isEditor)
            {
                newElement = Instantiate(_groundElementPrefab, 
                                transform.position, 
                                Quaternion.identity, transform.parent);
            }
            else
            {
                newElement = PrefabUtility.InstantiatePrefab(_groundElementPrefab, transform) 
                    as GroundElement;
                if(newElement == null) return;
            }
            
            newElement.SetData(groundElementData, -(int)transform.position.y);
            data.ground = newElement.GroundElementData;
            _groundElement = newElement;
        }
        
        public void ChangeGroundElementState(bool state)
        {
            _groundElement.gameObject.SetActive(state);
        }
        
        [ContextMenu(nameof(UpdateGroundElement))]
        public void UpdateGroundElement()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            AddGroundElement(_objectContext.groundElements[(int)_terrainType], true);
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
        public SoilStrength[] soils;

        public CellData()
        {
            decorations = new BuildElementData[2];
        }
    }
}