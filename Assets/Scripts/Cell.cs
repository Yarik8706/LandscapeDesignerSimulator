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

    [field: SerializeField] public SoilStrength SoilStrength { get; private set; } = SoilStrength.Strong;
    [field: SerializeField] public DirtType DirtType { get; private set; } = DirtType.Good;

    [SerializeField] private TerrainType _terrainType;
    [SerializeField] private ObjectContext _objectContext;
    [SerializeField] private GroundElement _groundElementPrefab;

    private BuildElement[] _buildElements = new BuildElement[2];

    private void Start()
    {
        data.ground = _objectContext.groundElements[(int)_terrainType];
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        GetComponent<SpriteRenderer>().sortingOrder = -100;

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
        if (_buildElements[(int)buildElementData.category] != null 
            && data.decorations[(int)buildElementData.category].id == buildElementData.id)
        {
            return;
        }
        if (_buildElements[(int)buildElementData.category] != null)
        {
            Destroy(_buildElements[(int)buildElementData.category].gameObject);
            data.decorations[(int)buildElementData.category] = null;
        }
        var oppositeIndex = OppositeIndex((int)buildElementData.category);
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

        data.decorations[(int)buildElementData.category] = newElement.BuildElementData;
        _buildElements[(int)buildElementData.category] = newElement;

        ChangeGroundElementState(false);
        ProjectCalculator.Instance.CalculateCurrentTerritory();

        AudioManager.Instance?.PlayPlaceSound();
    }

    public void RemoveBuildElement(Category category, bool returnHalfCost = false)
    {
        var buildElementData = data.decorations[(int)category];
        if (buildElementData == null) return;
        Destroy(_buildElements[(int)category].gameObject);
        data.decorations[(int)category] = null;
        _buildElements[(int)category] = null;
        if (returnHalfCost)
        {
            ProjectCalculator.Instance.AddValues(
                buildElementData.cost / 2,
                buildElementData.buildTime / 2);
        }

        if (_buildElements[0] == null && _buildElements[1] == null)
        {
            ChangeGroundElementState(true);
        }

        ProjectCalculator.Instance.CalculateCurrentTerritory();
    }

    public void AddGroundElement(GroundElementData groundElementData)
    {
        if (_groundElement != null)
        {
            Destroy(_groundElement.gameObject);
        }

        var newElement = Instantiate(_groundElementPrefab, transform.position, Quaternion.identity, transform);

        SetupGroundElement(newElement, groundElementData);
    }

    public void BreakDecoration()
    {
        RemoveBuildElement(Category.Decoration, true);
    }

#if UNITY_EDITOR
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

    public void AddGroundElementInEditor(GroundElementData groundElementData)
    {
        if (_groundElement != null && _groundElement.GroundElementData.id == groundElementData.id) return;
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
#endif

    private void SetupGroundElement(GroundElement element, GroundElementData data)
    {
        element.SetData(data, -(int)transform.position.y * 10);
        this.data.ground = element.GroundElementData;
        _groundElement = element;
    }

    private void ChangeGroundElementState(bool state)
    {
        _groundElement.gameObject.SetActive(state);
    }

    [ContextMenu(nameof(SetRandomSoilStrengthAndDirtType))]
    public void SetRandomSoilStrengthAndDirtType()
    {
        SoilStrength = Random.Range(0, 4) == 0 ? SoilStrength.Weak : SoilStrength.Strong;
        DirtType = Random.Range(0, 4) == 0 ? DirtType.Bad : DirtType.Good;
    }

    public void Click(int button)
    {
        if (button == 0) BuildSystem.Instance.ClickOnCell(this);
        else if (button == 1) BuildSystem.Instance.ResetCell(this);
    }
}

[Serializable]
public class CellData
{
    public int x;
    public int y;
    public GroundElementData ground;
    public BuildElementData[] decorations;

    public CellData()
    {
        decorations = new BuildElementData[2];
    }
}
