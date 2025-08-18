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

        private void Start()
        {
            GameDataManager.OnGameStageChanged.AddListener(OnStageChanged);
            foreach (var transition in _startHideTransitions)
            {
                transition.ChangeActiveInstantly(false);
            }
        }

        private void OnStageChanged()
        {
            foreach (var transition in _stages[(int)GameDataManager.Instance.gameData.stage].transitions)
            {
                transition.elementTransition.ChangeActive(transition.state);
            }
        }
    }
}