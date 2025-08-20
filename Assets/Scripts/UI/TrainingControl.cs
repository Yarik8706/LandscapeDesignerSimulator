using System;
using System.Linq;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [Serializable]
    public class TrainingBlock
    {
        public GameStage stage;
        public int clientIndex;
        public Transform position;
        [TextArea] public string description;
    }
    
    public class TrainingControl : MonoBehaviour
    {
        [SerializeField] private TrainingBlock[] trainingBlocks;
        [SerializeField] private Button _hideTrainingButton;
        [SerializeField] private Button _showTrainingButton;
        [SerializeField] private Transform _trainingPanel;
        [SerializeField] private Transform _hidePosition;
        [SerializeField] private TMP_Text _description;

        private int _currentBlockIndex;
        
        public static TrainingControl Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
            _hideTrainingButton.onClick.AddListener(HideTrainingPanel);
            _showTrainingButton.onClick.AddListener(ShowTrainingPanel);
            GameDataManager.OnGameStageChanged.AddListener(OnNextStage);
            _trainingPanel.transform.position = _hidePosition.position;
        }

        public void OnNextStage()
        {
            var trainingBlock = trainingBlocks.FirstOrDefault(
                x => x.stage == GameDataManager.Instance.gameData.stage 
                     && x.clientIndex == GameDataManager.Instance.gameData.activeClientIndex);
            if (trainingBlock == null)
            {
                HideTrainingPanel();
                return;
            }
            _currentBlockIndex = Array.IndexOf(trainingBlocks, trainingBlock);
            ShowTrainingPanel();
        }
        
        public void HideTrainingPanel()
        {
            if (GameDataManager.Instance.gameData.stage == GameStage.FirstDialog)
            {
                GameStageTransitionsUI.Instance.OnStageChanged();
            };
            _trainingPanel.ChangeActive(_hidePosition.position);
            _showTrainingButton.gameObject.SetActive(true);
        }

        public void ShowTrainingPanel()
        {
            _trainingPanel.ChangeActive(trainingBlocks[_currentBlockIndex].position.position);
            _description.text = trainingBlocks[_currentBlockIndex].description;
            _showTrainingButton.gameObject.SetActive(false);
        }
    }
}