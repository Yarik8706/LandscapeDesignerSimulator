using DefaultNamespace;
using UnityEngine;

public class BuildSystem : MonoBehaviour
{
    [SerializeField] private BuildElement _buildElementPrefab;
    [SerializeField] private GameObject _selectPoint;
        
    private BuildElementData _buildElementData;
        
    public static BuildSystem Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        var cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Mathf.Abs(cursorPos.x) > 5 || Mathf.Abs(cursorPos.y) > 5)
        {
            _selectPoint.SetActive(false);
            return;
        }
        _selectPoint.SetActive(true);
        _selectPoint.transform.position = new Vector3(Mathf.RoundToInt(cursorPos.x), 
            Mathf.RoundToInt(cursorPos.y), 0);
    }

    public void SelectObject(BuildElementData buildElementData)
    {
        _buildElementData = buildElementData;
    }
        
    public void ClickOnCell(Cell cell)
    {
        if (_buildElementData == null)
        {
            CellContextMenuUI.Instance.Show(cell);
            return;
        }
        if(cell.data.decorations[(int)_buildElementData.category] != null) return;
        CreateElement(_buildElementData, cell);
        _buildElementData = null;
    }
        
    public void CreateElement(BuildElementData buildElementData, Cell cell)
    {
        }
}