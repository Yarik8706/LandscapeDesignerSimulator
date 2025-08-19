using System;
using DefaultNamespace;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class GameStageText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _gameStageText;
        [SerializeField] private ElementTransition _transition;
        
        private void Start()
        {
            GameDataManager.OnGameStageChanged.AddListener(OnStageChanged);
            _transition.ChangeActiveInstantly(false);
            OnStageChanged();
        }

        private void OnStageChanged()
        {
            _transition.ChangeActive(false).OnComplete(() =>
            {
                _transition.ChangeActive(true);
                _gameStageText.text = GameDataManager.Instance.gameData.stage switch
                {
                    GameStage.FirstDialog => "Первый разговор с клиентом",
                    GameStage.Creating => "Создание макета",
                    GameStage.SecondDialog => "Правки",
                    GameStage.FixMistakes => "Доработка",
                    GameStage.Building => "Строительство",
                    GameStage.FinalDialog => "Финальный разговор с клиентом",
                    _ => throw new ArgumentOutOfRangeException()
                };
            });
        }
    }
}