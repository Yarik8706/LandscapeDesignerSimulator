using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class Details
    {
        public int aesthetics;
        public int functionality;
        public int cost;
        public float time;
    }
    
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
            GameDataManager.OnGameStageChanged.AddListener(() =>
            {
                if (GameDataManager.Instance.gameData.stage == GameStage.FirstDialog) ClearValues();
            });
        }
        
        public Details GetValues() => new Details { aesthetics = aesthetics, functionality = functionality, cost = cost, time = time };
        
        public void AddValues(int aesthetics, int functionality, int cost, float time)
        {
            this.aesthetics += aesthetics;
            this.functionality += functionality;
            this.cost += cost;
            this.time += time;
            
            this.aesthetics = Mathf.Clamp(this.aesthetics, 0, 80);
            this.functionality = Mathf.Clamp(this.functionality, 0, 80);
            this.cost = this.cost < 0 ? 0 : this.cost;
            this.time = this.time < 0 ? 0 : this.time;
            
            aestheticsSlider.value = this.aesthetics/80f*100f;
            functionalitySlider.value = this.functionality/80f*100f;
            
            costText.text = "Бюджет проекта: " + this.cost.ToString();
            timeText.text = "Срок постройки: " + this.time.ToString(CultureInfo.InvariantCulture);
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