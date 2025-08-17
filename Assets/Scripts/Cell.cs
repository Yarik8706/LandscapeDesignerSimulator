using System;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public CellData data = new CellData();

    [SerializeField] private GroundElement _groundElement;
    [SerializeField] private BuildElement[] _buildElements;

    [SerializeField] private SoilStrength[] _soilsStrength;
    [SerializeField] private TerrainType _terrainType;
    [SerializeField] private ObjectContext _objectContext;
    [SerializeField] private BuildElement _buildElementPrefab;
    [SerializeField] private GroundElement _groundElementPrefab;

    private void Start()
    {
        data.ground = _objectContext.groundElements[(int)_terrainType];
        data.soils = _soilsStrength;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        AddGroundElement(_objectContext.groundElements[(int)_terrainType]);
    }

    private void OnValidate()
    {
        data.x = (int)transform.position.x;
        data.y = (int)transform.position.y;
        AddGroundElementInEditor(_objectContext.groundElements[(int)_terrainType]);
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
        
    public void AddGroundElement(GroundElementData groundElementData)
    {
        if(_groundElement != null && _groundElement.GroundElementData.id == groundElementData.id) return;
        if (_groundElement != null)
        {
            Destroy(_groundElement.gameObject);
        }
            
        var newElement = Instantiate(_groundElementPrefab, 
            transform.position, 
            Quaternion.identity, transform.parent);
            
        SetupGroundElement(newElement, groundElementData);
    }
        
    public void AddGroundElementInEditor(GroundElementData groundElementData)
    {
        if(_groundElement != null && _groundElement.GroundElementData.id == groundElementData.id) return;
        if (_groundElement != null)
        {
            // In editor, we don't destroy the previous element immediately
            // as it might be part of the prefab instance
        }
            
        var newElement = PrefabUtility.InstantiatePrefab(_groundElementPrefab, transform) as GroundElement;
        if (newElement == null) return;
            
        newElement.transform.position = transform.position;
        SetupGroundElement(newElement, groundElementData);
    }
        
    private void SetupGroundElement(GroundElement element, GroundElementData data)
    {
        element.SetData(data, -(int)transform.position.y);
        this.data.ground = element.GroundElementData;
        _groundElement = element;
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
        AddGroundElementInEditor(_objectContext.groundElements[(int)_terrainType]);
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