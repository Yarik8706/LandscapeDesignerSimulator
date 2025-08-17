using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class ObjectManager : MonoBehaviour
    {
        public static ObjectManager Instance { get; private set; }

        public ObjectContext objectContext;
        
        private void Awake()
        {
            Instance = this;
        }
    }

    [Serializable]
    public class SoilStrengthDescription
    {
        public SoilStrength strength;
        public string description;
    }
}