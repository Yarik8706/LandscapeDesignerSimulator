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
                GameDataManager.Instance.gameData.stage = GameStage.FirstDialog;
                GameDataManager.OnGameStageChanged.Invoke();
                return;
            };
            GameDataManager.Instance.gameData.stage++;
            GameDataManager.OnGameStageChanged.Invoke();
        }
    }
}