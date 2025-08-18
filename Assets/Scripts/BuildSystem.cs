using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class BuildSystem : MonoBehaviour
{
    [SerializeField] private BuildElement _buildElementPrefab;
    [SerializeField] private GameObject _selectPoint;
    [SerializeField] private Button _resetCellButton;
    [SerializeField] private Sprite[] _resetCellButtonSprites;
        
    private BuildElementData _buildElementData;
    private bool _isResetCellMode;
        
    public static BuildSystem Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        _resetCellButton.onClick.AddListener(() =>
            {
                _isResetCellMode = !_isResetCellMode;
                _resetCellButton.image.sprite = _isResetCellMode ? _resetCellButtonSprites[1] 
                    : _resetCellButtonSprites[0];
            });
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
        if (GameDataManager.Instance.gameData.stage != 
            GameStage.Building && GameDataManager.Instance.gameData.stage != GameStage.FixMistakes) return;
        if (_isResetCellMode)
        {
            ResetCell(cell);
            return;
        }
        if (_buildElementData == null)
        {
            CellContextMenuUI.Instance.Show(cell);
            return;
        }
        if(cell.data.decorations[(int)_buildElementData.category] != null
           || !_buildElementData.terraform.overlayOn.Contains(cell.data.ground.id)
           ) return;
        CreateElement(_buildElementData, cell);
        _buildElementData = null;
    }
    
    public void ResetCell(Cell cell)
    {
        cell.RemoveBuildElement(Category.Decoration);
        cell.RemoveBuildElement(Category.Embankment);
    }
        
    public void CreateElement(BuildElementData buildElementData, Cell cell)
    {
        cell.AddBuildElement(buildElementData);
    }
}