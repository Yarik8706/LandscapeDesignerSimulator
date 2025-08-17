using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour {
  [SerializeField] private ChatService chatService;
  [SerializeField] private ProgressTracker tracker;
  [SerializeField] private TextMeshProUGUI statusText;
  [SerializeField] private GameObject assistantBubblePrefab;

  public async void Send(string message) {
    var req = new ChatRequest { message = message };
    var resp = await chatService.Send(req);
    AppendBubble(assistantBubblePrefab, resp.reply);
    tracker?.UpdateFromResponse(resp.progress);
    if (resp.lowConfidence) statusText.text = "⚠ Низкая уверенность ответа";
  }

  private void AppendBubble(GameObject prefab, string text) { }
}

