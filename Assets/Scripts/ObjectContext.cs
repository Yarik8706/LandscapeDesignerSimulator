using UnityEngine;

namespace DefaultNamespace
{
    public class ObjectContext : ScriptableObject
    {
        public ConstructionObject[] constructionObjects;
        public SoilStrengthDescription[] soilStrengthDescriptions;
        public GroundElementData[] groundElements;
    }
}