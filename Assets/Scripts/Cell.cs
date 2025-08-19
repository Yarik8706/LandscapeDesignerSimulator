using System;
using System.Linq;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Cell : MonoBehaviour
{
    public CellData data = new CellData();

    [SerializeField] private GroundElement _groundElement;
    
    [SerializeField] private SoilStrength[] _soilsStrength;
    [SerializeField] private TerrainType _terrainType;
    [SerializeField] private ObjectContext _objectContext;
    [SerializeField] private GroundElement _groundElementPrefab;
    
    private BuildElement[] _buildElements = new BuildElement[2];

    private void Start()
    {
        data.ground = _objectContext.groundElements[(int)_terrainType];
        data.soils = _soilsStrength;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        AddGroundElement(_objectContext.groundElements[(int)_terrainType]);
        data.x = (int)transform.position.x;
        data.y = (int)transform.position.y;
    }

    private void OnValidate()
    {
        data.x = (int)transform.position.x;
        data.y = (int)transform.position.y;
    }
    
    private int OppositeIndex(int index)
    {
        return index == 0 ? 1 : 0;
    }

    public void AddBuildElement(BuildElementData buildElementData)
    {
        var oppositeIndex = OppositeIndex((int)buildElementData.category);
        if(data.decorations[(int)buildElementData.category] != null
           || !buildElementData.terraform.overlayOn.Contains(TerrainType.All)
           && buildElementData.terraform.overlayOn.Count != 0
           && !buildElementData.terraform.overlayOn.Contains(data.ground.id)
           ) return;
        Debug.Log("AddBuildElement " + buildElementData.name);
        // logs need and avoid
       Debug.Log(data.decorations[oppositeIndex] != null &&
                  (data.decorations[oppositeIndex].constraints.proximity.avoid.Contains(buildElementData)
                   || buildElementData.constraints.proximity.avoid.Contains(data.decorations[oppositeIndex])));
        Debug.Log(data.decorations[oppositeIndex] != null && 
                  (!data.decorations[oppositeIndex].constraints.proximity.need.Contains(buildElementData)
                   || !buildElementData.constraints.proximity.need.Contains(data.decorations[oppositeIndex])));
        if (data.decorations[oppositeIndex] != null && 
            (data.decorations[oppositeIndex].constraints.proximity.avoid.Contains(buildElementData)
             || buildElementData.constraints.proximity.avoid.Contains(data.decorations[oppositeIndex])))
            return;
        if (data.decorations[oppositeIndex] != null && 
            data.decorations[oppositeIndex].constraints.proximity.need.Count != 0 &&
            buildElementData.constraints.proximity.need.Count != 0 &&
            (!data.decorations[oppositeIndex].constraints.proximity.need.Contains(buildElementData)
             || !buildElementData.constraints.proximity.need.Contains(data.decorations[oppositeIndex])))
            return;
        var newElement = Instantiate(buildElementData.prefab, transform.position, Quaternion.identity, transform);
        newElement.SetData(buildElementData, -(int)transform.position.y);
        Debug.Log("AddBuildElement " + buildElementData.name);
        LandscapeProjectDetailsUI.Instance.AddValues(
            buildElementData.delta.A,
            buildElementData.delta.F,
            buildElementData.cost,
            buildElementData.buildTime);

        data.decorations[(int)buildElementData.category] = newElement.BuildElementData;
        _buildElements[(int)buildElementData.category] = newElement;
        if (buildElementData.terraform.overlayOn.Contains(data.ground.id))
        {
            LandscapeProjectDetailsUI.Instance.AddValues(
                0,
                0,
                data.ground.overlayCost,
                data.ground.overlayTime);
        }
        else
        {
            LandscapeProjectDetailsUI.Instance.AddValues(
                0,
                0,
                data.ground.replaceCost,
                data.ground.replaceTime);
        }

        ChangeGroundElementState(false);
    }

    public void RemoveBuildElement(Category category)
    {
        var buildElementData = data.decorations[(int)category];
        if (buildElementData == null) return;
        Destroy(_buildElements[(int)category].gameObject);
        data.decorations[(int)category] = null;
        _buildElements[(int)category] = null;
        LandscapeProjectDetailsUI.Instance.AddValues(
            -buildElementData.delta.A, 
            -buildElementData.delta.F,
            -buildElementData.cost,
            -buildElementData.buildTime);
        if (data.decorations[OppositeIndex((int)category)] == null)
        {
            if (buildElementData.terraform.overlayOn.Contains(data.ground.id))
            {
                LandscapeProjectDetailsUI.Instance.AddValues(
                    0, 
                    0,
                    -data.ground.overlayCost,
                    -data.ground.overlayTime);
            }
            else
            {
                LandscapeProjectDetailsUI.Instance.AddValues(
                    0, 
                    0,
                    -data.ground.replaceCost,
                    -data.ground.replaceTime);
            }
        }
        
        if (_buildElements[0] == null && _buildElements[1] == null)
        {
            ChangeGroundElementState(true);
        }
    }
        
    public void AddGroundElement(GroundElementData groundElementData)
    {
        if(_groundElement != null && _groundElement.GroundElementData != null
               && _groundElement.GroundElementData.id == groundElementData.id) return;
        if (_groundElement != null)
        {
            Destroy(_groundElement.gameObject);
        }
            
        var newElement = Instantiate(_groundElementPrefab, transform.position, Quaternion.identity, transform);
            
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
    
    [ContextMenu(nameof(SetRandomGroundElement))]
    public void SetRandomGroundElement()
    {
        _terrainType = (TerrainType)Random.Range(0, _objectContext.groundElements.Length);
        UpdateGroundElement();
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

    private void OnMouseUp()
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