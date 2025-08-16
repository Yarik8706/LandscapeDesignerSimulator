using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class BuildElementDetailsUI : MonoBehaviour
    {
        public static BuildElementDetailsUI Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void Show(BuildElementData buildElementData)
        {
            
        }
    }
}