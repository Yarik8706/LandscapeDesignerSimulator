using System;
using System.Linq;
using DefaultNamespace;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

[Serializable]
public class Level
{
    public string name;
    public MapData prefab;
}

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private Level[] _levels;
    [SerializeField] private Transform _levelContainer;
    [SerializeField] private GameObject _gameEndWindow;

    public MapData CurrentLevel { get; private set; }

    private int[] _levelsOrder;
    private int _currentLevelIndex = -1;
    
    public static LevelSelector Instance { get; private set; }
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        _levelsOrder = Enumerable.Range(0, _levels.Length).ToArray();
        NextLevel();
    }

    public void ResetLevel()
    {
        if(CurrentLevel != null) Destroy(CurrentLevel.gameObject);
        GameDataManager.Instance.gameData.stage = GameStage.FirstDialog;
        CurrentLevel = Instantiate(_levels[_levelsOrder[_currentLevelIndex]].prefab, _levelContainer);
        GameDataManager.Instance.gameData.activeClientBrief = _levelsOrder[_currentLevelIndex];
        GameDataManager.OnGameStageChanged.Invoke();
    }
    
    public void NextLevel()
    {
        _currentLevelIndex++;
        if (_currentLevelIndex >= _levels.Length)
        {
            TrainingControl.Instance.HideTrainingPanel();
            _gameEndWindow.SetActive(true);
            return;
        }
        
        ResetLevel();
    }

}