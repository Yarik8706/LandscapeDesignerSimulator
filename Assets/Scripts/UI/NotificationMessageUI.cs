using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class NotificationMessageUI : MonoBehaviour
    {

        [SerializeField] private ElementTransition _transition;
        [SerializeField] private TMP_Text _messageText;
        
        public static NotificationMessageUI Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }

        public void ShowMessage(string message, float duration)
        {
            _messageText.text = message;
            _transition.ChangeActive(true).OnComplete(() =>
            {
                DOVirtual.DelayedCall(duration, () => _transition.ChangeActive(false));
            });
        }
    }
}