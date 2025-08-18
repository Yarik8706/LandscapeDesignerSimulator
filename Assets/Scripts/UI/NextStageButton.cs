using DefaultNamespace;
using UnityEngine;

namespace UI
{
    public class NextStageButton : MonoBehaviour
    {
        public void NextStage()
        {
            GameDataManager.OnGameStageChanged.Invoke();
            if (GameDataManager.Instance.gameData.stage == GameStage.FinalDialog)
            {
                GameDataManager.Instance.gameData.stage = GameStage.FirstDialog;
                return;
            };
            GameDataManager.Instance.gameData.stage++;
        }
    }
}