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
        
        public static LandscapeProjectDetailsUI Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
            GameDataManager.OnGameStageChanged.AddListener(() =>
            {
                if (GameDataManager.Instance.gameData.stage == GameStage.FirstDialog) ClearValues();
            });
        }
        
        public void SetValues(int aesthetics, int functionality, int cost, float time)
        {
            aesthetics = Mathf.Clamp(aesthetics, 0, 80);
            functionality = Mathf.Clamp(functionality, 0, 80);
            cost = cost < 0 ? 0 : cost;
            time = time < 0 ? 0 : time;
            
            aestheticsSlider.value = aesthetics/30f;
            functionalitySlider.value = functionality/30f;
            
            costText.text = "Бюджет проекта: " + cost.ToString() + " $";
            timeText.text = "Срок постройки (дней): " + time.ToString(CultureInfo.InvariantCulture);
        }
        
        public void ClearValues()
        {
            aestheticsSlider.value = 0;
            functionalitySlider.value = 0;
            
            costText.text = "Бюджет проекта: " + 0;
            timeText.text = "Срок постройки: " + 0;
        }
    }
}