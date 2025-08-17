using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "ObjectContext", menuName = "ScriptableObjects/ObjectContext")]
    public class ObjectContext : ScriptableObject
    {
        public ConstructionObject[] constructionObjects;
        public SoilStrengthDescription[] soilStrengthDescriptions;
        public GroundElementData[] groundElements;
    }
}