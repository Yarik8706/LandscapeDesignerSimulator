using DefaultNamespace;
using UnityEngine;

namespace UI
{
    public class NextStageButton : MonoBehaviour
    {
        public void NextStage()
        {
            if (GameDataManager.Instance.gameData.stage == GameStage.FinalDialog)
            {
                DialogueUI.Instance.PlayerEndProject();
                return;
            };
            GameDataManager.Instance.gameData.stage++;
            GameDataManager.OnGameStageChanged.Invoke();
        }
    }
}