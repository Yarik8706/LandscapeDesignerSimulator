using System;
using System.Linq;
using DefaultNamespace;
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
        FirebaseInitializer.OnInitialized.AddListener(NextLevel);
    }

    public void NextLevel()
    {
        _currentLevelIndex++;
        if (_currentLevelIndex >= _levels.Length)
        {
            _gameEndWindow.SetActive(true);
            return;
        }
        if(CurrentLevel != null) Destroy(CurrentLevel);
        CurrentLevel = Instantiate(_levels[_levelsOrder[_currentLevelIndex]].prefab, _levelContainer);
        GameDataManager.Instance.gameData.activeClientBrief = _levelsOrder[_currentLevelIndex];
        GameDataManager.OnGameStageChanged.Invoke();
    }

}

public static class ArrayExtensions
{
    private static readonly Random rng = new Random();

    public static void Shuffle<T>(this T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (array[k], array[n]) = (array[n], array[k]);
        }
    }
}