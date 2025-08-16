using UnityEngine;

namespace DefaultNamespace
{
    public class GroundElementData : ScriptableObject
    {
        public string displayName;
        public Sprite icon;
        public TerrainType id;
        public SoilStrength[] soils;
        
        public int replaceCost;
        public int replaceTime;
        public int overlayCost;
        public int overlayTime;
    }
}

public enum SoilStrength
{
    Clayey,
    Loamy,
    Sandy,
    Lime,
    Peat,
    Chernozem
}