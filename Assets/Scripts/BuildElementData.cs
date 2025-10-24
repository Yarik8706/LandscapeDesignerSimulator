using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildElement", menuName = "LD/Build Element", order = 0)]
public class BuildElementData : ScriptableObject
{
    [Header("Identity")]
    public int id;
    public string displayName;                 // имя для UI
    [TextArea]
    public string additionalDescription;                 // описание
    public Category category = Category.Decoration;
    public ElementType elementType = ElementType.Structure;
    public BuildElement prefab;
    
    [Header("Visuals")]
    public Sprite icon;
    
    [Header("Economy")]
    public int cost = 25;                    // базовая стоимость
    public float buildTime = 1f;              // базовое время постройки (игровые единицы)

    [Header("Scoring Delta")]
    public DeltaScore delta = new () { F = 0, A = 0, S = 0 };

    [Header("Stability")]
    public Stability stability = new ();

    [Header("Constraints")]
    public Constraints constraints = new ();

    [Header("Adjacency (bonuses)")]
    public List<AdjacencyRule> adjacency = new ();

    [Header("Terraform Policy")]
    public Terraform terraform = new ();
    
    public static string GetDescription(BuildElementData buildElementData)
    {
        var text = $"Стоимость ({GameDataManager.Instance.gameData.costTypeText}): {buildElementData.cost}\n";
        text += $"Время постройки (дней): {buildElementData.buildTime}\n";
        text += $"Функциональность: {buildElementData.delta.F}\n";
        text += $"Эстетика: {buildElementData.delta.A}\n";
        if (buildElementData.terraform.overlayOn.Count > 0)
        {
            text += $"Уменьшает стоимость терраформирования следующих клеток: ";
            foreach (var t in buildElementData.terraform.overlayOn)
            {
                text += $"{ObjectManager.Instance.objectContext.groundElements.First(x => x.id == t).displayName}, ";
            }
            text = text.Remove(text.Length - 2);
        }
        
        return text;
    }
}

/* ==== ENUMS ==== */
public enum Category { 
    Embankment,
    Decoration,
}
public enum TerrainType { 
    Pound, 
    Swamp, 
    Forest, 
    Steppe, 
    Mountain,
}
public enum AuraShape { None, Circle, Square }
public enum ElementType { 
    Plants,
    Structure,
}
/* ==== DATA BLOCKS ==== */

[Serializable]
public struct DeltaScore
{
    public int F;   // функциональность
    public int A;   // эстетика
    public int S;   // устойчивость (вклад в общий S зоны)
}

[Serializable]
public class Stability
{
    [Tooltip("Допуски к климату (+/- к окнам допустимых значений)")]
    public Tolerance tolerance = new Tolerance { temp = 0, wind = 0, hum = 0 };

    [Range(0.1f, 2f)]
    [Tooltip("Множитель к шансам дефекта при стройке (<1 лучше)")]
    public float failMod = 1f;
}

[Serializable]
public class Tolerance
{
    public float temp; // °C
    public float wind; // м/с
    public float hum;  // %
}

[Serializable]
public class Constraints
{
    [Tooltip("Разрешённые базовые типы террейна")]
    public List<TerrainType> terrainAllowed = new List<TerrainType> {  };
    
    [Tooltip("Климатические окна допустимости")]
    public ClimateWindow climate = new ClimateWindow
    {
        temp = new MinMaxFloat(-25, 45),
        hum = new MinMaxFloat(0, 95),
        windMax = 25f
    };

    [Tooltip("Требуемая близость к объектам и запрещённые соседства")]
    public Proximity proximity = new Proximity();
}

[Serializable]
public struct MinMaxInt
{
    public int min;
    public int max;
    public MinMaxInt(int min, int max) { this.min = min; this.max = max; }
}

[Serializable]
public struct MinMaxFloat
{
    public float min;
    public float max;
    public MinMaxFloat(float min, float max) { this.min = min; this.max = max; }
}

[Serializable]
public class ClimateWindow
{
    public MinMaxFloat temp;
    public MinMaxFloat hum;    // влажность %
    public float windMax;      // м/с (верхняя граница)
}

[Serializable]
public class Proximity
{
    [Tooltip("Нужно наличие указанных объектов в радиусе")]
    public List<BuildElementData> need = new();
    
    [Tooltip("Запрещено наличие указанных объектов в радиусе")]
    public List<BuildElementData> avoid = new ();
}

[Serializable]
public class AdjacencyRule
{
    [Tooltip("Требуемый объект по соседству для бонуса (например, path)")]
    public string needId;
    public DeltaScore bonus; // прибавки к F/A/S при выполнении условия
}

[Serializable]
public class Terraform
{
    [Tooltip("Поверхности, поверх которых можно класть объект без удаления (Overlay)")]
    public List<TerrainType> overlayOn = new List<TerrainType>(){};
}

