using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildElement", menuName = "LD/Build Element", order = 0)]
public class BuildElementData : ScriptableObject
{
    [Header("Identity")]
    public int id;                         
    public string displayName;                 // имя для UI
    public Category category = Category.Decoration;
    
    [Header("Visuals")]
    public Sprite icon;
    
    [Header("Economy")]
    public int cost = 25;                    // базовая стоимость
    public float buildTime = 1f;              // базовое время постройки (игровые единицы)

    [Header("Scoring Delta")]
    public DeltaScore delta = new DeltaScore { F = 0, A = 0, S = 0 };

    [Header("Stability")]
    public Stability stability = new Stability();

    [Header("Constraints")]
    public Constraints constraints = new Constraints();

    [Header("Adjacency (bonuses)")]
    public List<AdjacencyRule> adjacency = new List<AdjacencyRule>();

    [Header("Terraform Policy")]
    public Terraform terraform = new Terraform();
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
    All,
    None
}
public enum AuraShape { None, Circle, Square }

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

    [Tooltip("Аура устойчивости/микроклимата")]
    public Aura aura = new Aura();
}

[Serializable]
public class Tolerance
{
    public float temp; // °C
    public float wind; // м/с
    public float hum;  // %
}

[Serializable]
public class Aura
{
    public int radius;                 // радиус в тайлах
    public AuraShape shape = AuraShape.None;
    [Tooltip("Эффекты ауры (например, heat:-3, wind:-2, s_local:+5)")]
    public List<EffectEntry> effects = new List<EffectEntry>();
}

[Serializable]
public class EffectEntry
{
    public string key; // "heat","wind","hum","s_local" и т.п.
    public float value;
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
    
    [Tooltip("Допустимые объекты поверх которых разметить этот объект")]
    public List<BuildElementData> allowedBase = new List<BuildElementData> {  };
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
    public List<ProximityNeed> need = new List<ProximityNeed>();
    
    [Tooltip("Запрещено наличие указанных объектов в радиусе")]
    public List<ProximityAvoid> avoid = new List<ProximityAvoid>();
}

[Serializable]
public class ProximityNeed
{
    public int id;   // например, "path"
    public int radius = 1;
}

[Serializable]
public class ProximityAvoid
{
    public int id;   // например, "bbq"
    public int radius = 2;
}

[Serializable]
public class AdjacencyRule
{
    [Tooltip("Требуемый объект по соседству для бонуса (например, path)")]
    public string needId;
    public int radius = 1;
    public DeltaScore bonus; // прибавки к F/A/S при выполнении условия
}

[Serializable]
public class Terraform
{
    [Tooltip("Поверхности, поверх которых можно класть объект без удаления (Overlay)")]
    public List<TerrainType> overlayOn = new List<TerrainType>(){TerrainType.All};
}