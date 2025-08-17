using TMPro;
using UnityEngine;

public class ProgressTracker : MonoBehaviour {
  [SerializeField] private TextMeshProUGUI label;
  private const int Total = 8; // budget, deadline, weights, must, bans, climate, risks, bonus

  public void UpdateFromResponse(ProgressData p) {
    if (p == null) { label.text = $"Выяснено: 0/{Total}"; return; }
    int n = 0;
    if (p.budget) n++;
    if (p.deadline) n++;
    if (p.weights) n++;
    if (p.must) n++;
    if (p.bans) n++;
    if (p.climate) n++;
    if (p.risks) n++;
    if (p.bonus) n++;
    label.text = $"Выяснено: {n}/{Total}";
  }
}

