using System;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class GameDataManager : MonoBehaviour
    {
        public GameData gameData;
        
        public static GameDataManager Instance { get; private set; }
        
        public static UnityEvent OnGameStageChanged = new UnityEvent();
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            gameData.stage = GameStage.FirstDialog;
            FirebaseInitializer.OnInitialized.AddListener(() =>
                OnGameStageChanged.Invoke());

        }
    }
}