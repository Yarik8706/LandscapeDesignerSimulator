using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "ObjectContext", menuName = "ScriptableObjects/ObjectContext")]
    public class ObjectContext : ScriptableObject
    {
        public BuildElementData[] buildElement;
        public SoilStrengthDescription[] soilStrengthDescriptions;
        public GroundElementData[] groundElements;
    }
}