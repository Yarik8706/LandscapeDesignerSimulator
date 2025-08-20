using System;
using UnityEngine;

namespace DefaultNamespace
{

    public enum GameStage
    {
        FirstDialog,
        Creating,
        SecondDialog,
        FixMistakes,
        Building,
        FinalDialog
    }
    
    [Serializable]
    [CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData")]
    public class GameData : ScriptableObject
    {
        public int activeClientIndex = 0;
        public int activeClientBrief = 0;
        public int firstDialogQuestionCount = 8;
        public int secondDialogQuestionCount = 5;
        public GameStage stage = GameStage.FirstDialog;

        public AudioClip placeSound;
        public AudioClip destroySound;
        public AudioClip backgroundMusic;
    }
}
