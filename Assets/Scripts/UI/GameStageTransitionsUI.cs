using System;
using DefaultNamespace;
using UnityEngine;

namespace UI
{
    [Serializable]
    public class Transition
    {
        public ElementTransition elementTransition;
        public bool state;
    }

    [Serializable]
    public class Stage
    {
        public Transition[] transitions;
    }
    
    public class GameStageTransitionsUI : MonoBehaviour
    {
        [SerializeField] private Stage[] _stages;
        [SerializeField] private ElementTransition[] _startHideTransitions;
        
        public static GameStageTransitionsUI Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GameDataManager.OnGameStageChanged.AddListener( () =>
            {
                if (GameDataManager.Instance.gameData.stage == GameStage.FirstDialog)
                {
                    foreach (var transition in _startHideTransitions)
                    {
                        transition.ChangeActiveInstantly(false);
                    }
                    return;
                };
                OnStageChanged();
            });
            foreach (var transition in _startHideTransitions)
            {
                transition.ChangeActiveInstantly(false);
            }
        }

        public void OnStageChanged()
        {
            foreach (var transition in _stages[(int)GameDataManager.Instance.gameData.stage].transitions)
            {
                transition.elementTransition.ChangeActive(transition.state);
            }
        }

#if UNITY_EDITOR
        [SerializeField] private GameStage _stage = GameStage.FirstDialog;
        
        [ContextMenu("Show Stage")]
        public void ShowStage()
        {
            foreach (var transition in _stages[(int)_stage].transitions)
            {
                transition.elementTransition.ChangeActiveInstantly(transition.state);
            }
        }
#endif
    }
}