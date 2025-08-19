using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class BuildSystem : MonoBehaviour
{
    [SerializeField] private GameObject _selectPoint;
    [SerializeField] private Button _resetCellButton;
    [SerializeField] private Button _unselectButton;
    [SerializeField] private Sprite[] _resetCellButtonSprites;
    [SerializeField] private Image _selectedElementImage;
    [SerializeField] private Sprite _defaultElementImage;
        
    private BuildElementData _buildElementData;
    private bool _isResetCellMode;
    private Camera _camera;
        
    public static BuildSystem Instance { get; private set; }

    private void Awake()
    {
        _camera = Camera.main;
        Instance = this;
        _resetCellButton.onClick.AddListener(() =>
            {
                DeselectObject();
                _isResetCellMode = !_isResetCellMode;
                _resetCellButton.image.sprite = _isResetCellMode ? _resetCellButtonSprites[1] 
                    : _resetCellButtonSprites[0];
            });
        _unselectButton.onClick.AddListener(DeselectObject);
    }

    private void Update()
    {
        
        _selectPoint.SetActive(false);
        if(LevelSelector.Instance.CurrentLevel == null) return;
        var cursorPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        var pos = new Vector3(Mathf.RoundToInt(cursorPos.x), 
            Mathf.RoundToInt(cursorPos.y), 0);
        for (var i = 0; i < LevelSelector.Instance.CurrentLevel.Cells.Length; i++)
        {
            if (LevelSelector.Instance.CurrentLevel.Cells[i].transform.position == pos)
            {
                _selectPoint.SetActive(true);
                _selectPoint.transform.position = pos;
                return;
            }
        }
    }

    public void SelectObject(BuildElementData buildElementData)
    {
        _selectedElementImage.sprite = buildElementData.icon;
        _buildElementData = buildElementData;
    }
    
    public void DeselectObject()
    {
        _selectedElementImage.sprite = _defaultElementImage;
        _buildElementData = null;
    }
        
    public void ClickOnCell(Cell cell)
    {
        Debug.Log("ClickOnCell");

        if (GameDataManager.Instance.gameData.stage !=
            GameStage.Creating 
            && GameDataManager.Instance.gameData.stage != GameStage.Building
            && GameDataManager.Instance.gameData.stage != GameStage.FixMistakes)
        {
            CellContextMenuUI.Instance.Show(cell);
            return;
        };
        Debug.Log("ClickOnCell 1");
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
        Debug.Log("ClickOnCell 1");
        CreateElement(_buildElementData, cell);
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