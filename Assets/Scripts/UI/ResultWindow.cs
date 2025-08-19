using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ResultWindow : MonoBehaviour
    {
        public GameObject winPanel;
        public TMP_Text _resultText;
        
        public static ResultWindow Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
        
        public void Show(string resultText)
        {
            
        }
    }
}