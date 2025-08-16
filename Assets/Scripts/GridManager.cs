using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }
        
        public Transform[] gridPositions;
        public BuildElement[] buildElements;

        private void Awake()
        {
            Instance = this;
        }
    }
}