using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using TMPro;
using UI;
using UnityEngine;

public class BuildingStage : MonoBehaviour
{
    [SerializeField] private GameObject _breakEffectPrefab;
    [SerializeField] private GameObject _needChangeMarkerPrefab;

    private readonly List<GameObject> _spawnedMarkers = new();
    private readonly List<GameObject> _spawnedEffects = new();

    private void Start()
    {
        GameDataManager.OnGameStageChanged.AddListener(OnStartBuilding);
    }

    public void OnStartBuilding()
    {
        ClearSpawned();
        if (GameDataManager.Instance.gameData.stage != GameStage.Building) return;

        var level = LevelSelector.Instance.CurrentLevel;
        var cells = level.Cells;
        var broken = new List<Cell>();
        
        foreach (var cell in cells)
        {
            if (cell == null || cell.data == null) continue;

            // decorations[1] — объект зоны (растение/строение)
            var deco = cell.data.decorations != null && cell.data.decorations.Length > 1
                ? cell.data.decorations[1]
                : null;
            if (deco == null) continue;

            // decorations[0] — подсыпка (Embankment)
            var emb = cell.data.decorations != null && cell.data.decorations.Length > 0
                ? cell.data.decorations[0]
                : null;

            // Спец-правила подсыпки:
            // id==0 → чернозём → отменяет поломку всегда
            if (emb != null && emb.id == 0) continue;

            bool isBroken = false;

            if (deco.elementType == ElementType.Plants)
            {
                // Растения ломаются на плохой почве, кроме случая с чернозёмом (учтено выше)
                isBroken = (cell.DirtType == DirtType.Bad);
            }
            else // ElementType.Structure
            {
                // Строения ломаются на слабом основании, кроме усиления id==1
                bool reinforced = (emb != null && emb.id == 1);
                isBroken = (cell.SoilStrength == SoilStrength.Weak) && !reinforced;
            }

            if (isBroken)
                broken.Add(cell);
        }

        if (broken.Count == 0)
        {
            ShowMessage("Все постройки успешно установлены.");
            return;
        }

        foreach (var c in broken)
        {
            c.BreakDecoration();
            AudioManager.Instance?.PlayBreakSound();
            if (_breakEffectPrefab != null)
            {
                var fx = Instantiate(_breakEffectPrefab, c.transform.position, Quaternion.identity);
                _spawnedEffects.Add(fx);
            }
            if (_needChangeMarkerPrefab != null)
            {
                var mk = Instantiate(_needChangeMarkerPrefab, c.transform.position, Quaternion.identity, c.transform);
                _spawnedMarkers.Add(mk);
            }
        }

        ShowMessage("Во время строительства некоторые постройки сломались из-за условий почвы и " +
                    "теперь требуют замены. Возвращена часть стоимости.");
    }

    private void ShowMessage(string text)
    {
        NotificationMessageUI.Instance.ShowMessage(text, 7f);
    }
    
   

    private void ClearSpawned()
    {
        foreach (var go in _spawnedMarkers) if (go) Destroy(go);
        foreach (var go in _spawnedEffects) if (go) Destroy(go);
        _spawnedMarkers.Clear();
        _spawnedEffects.Clear();
    }
}
