using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class LandscapeProjectDetailsUI : MonoBehaviour
    {
        // Текстовые поля с значениями эстетики, функциональности, стоимости и срокам
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI timeText;
        
        [SerializeField] private Slider aestheticsSlider;
        [SerializeField] private Slider functionalitySlider;
        
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
            
            aestheticsSlider.value = this.aesthetics/80f*100f;
            functionalitySlider.value = this.functionality/80f*100f;
            
            costText.text = "Бюджет проекта: " + cost.ToString();
            timeText.text = "Срок постройки: " + time.ToString(CultureInfo.InvariantCulture);
        }
        
        public void ClearValues()
        {
            aesthetics = 0;
            functionality = 0;
            cost = 0;
            time = 0;
            
            aestheticsSlider.value = 0;
            functionalitySlider.value = 0;
            
            costText.text = "Бюджет проекта: " + cost.ToString();
            timeText.text = "Срок постройки: " + time.ToString(CultureInfo.InvariantCulture);
        }
    }
}