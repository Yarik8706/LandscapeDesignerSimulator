using System.Globalization;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class LandscapeProjectDetailsUI : MonoBehaviour
    {
        // Текстовые поля с значениями эстетики, функциональности, стоимости и срокам
        [SerializeField] private TextMeshProUGUI aestheticsText;
        [SerializeField] private TextMeshProUGUI functionalityText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI timeText;
        
        // числа, которые нужно вставить в текст
        private int aesthetics;
        private int functionality;
        private int cost;
        private float time;
        
        public static LandscapeProjectDetailsUI Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }
        
        public void AddValues(int aesthetics, int functionality, int cost, float time)
        {
            this.aesthetics += aesthetics;
            this.functionality += functionality;
            this.cost += cost;
            this.time += time;
            
            aestheticsText.text = aesthetics.ToString();
            functionalityText.text = functionality.ToString();
            costText.text = cost.ToString();
            timeText.text = time.ToString(CultureInfo.InvariantCulture);
        }
        
        public void ClearValues()
        {
            aesthetics = 0;
            functionality = 0;
            cost = 0;
            time = 0;
            
            aestheticsText.text = aesthetics.ToString() + "/80";
            functionalityText.text = functionality.ToString() + "/80";
            costText.text = cost.ToString();
            timeText.text = time.ToString(CultureInfo.InvariantCulture);
        }
    }
}