using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "New Ground Element", menuName = "Ground Element")]
    public class GroundElementData : ScriptableObject
    {
        public string displayName;
        public Sprite icon;
        public TerrainType id;
        
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