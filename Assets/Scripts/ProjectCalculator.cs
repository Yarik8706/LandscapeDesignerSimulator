using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class ProjectCalculator : MonoBehaviour
    {
        public int Budget { get; private set; }
        public float Time { get; private set; }
        public int Aesthetics { get; private set; }
        public int Functionality { get; private set; }
        
        public int extraCost { get; private set; }
        public float extraTime { get; private set; }
        
        public void Clear()
        {
            Budget = 0;
            Time = 0f;
            Aesthetics = 0;
            Functionality = 0;
            LandscapeProjectDetailsUI.Instance.ClearValues();
        }
        
        public static ProjectCalculator Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }
        
        public Details GetValues() => new Details { aesthetics = Aesthetics, functionality = Functionality, cost = Budget, time = Time };

        public void AddValues(int cost, float time)
        {
            extraCost += cost;
            extraTime += time;
            LandscapeProjectDetailsUI.Instance.SetValues(Aesthetics, Functionality, Budget+extraCost, extraTime+Time);
        }
        
        public void CalculateCurrentTerritory()
        {
            Budget = 0;
            Time = 0f;
            Aesthetics = 0;
            Functionality = 0;

            if (LevelSelector.Instance == null || LevelSelector.Instance.CurrentLevel == null) return;

            var cells = LevelSelector.Instance.CurrentLevel.Cells;
            foreach (var cell in cells)
            {
                var decorations = cell.data.decorations;
                var hasDecoration = false;
                var overlay = false;

                foreach (var decoration in decorations)
                {
                    if (decoration == null) continue;
                    hasDecoration = true;

                    Aesthetics += decoration.delta.A;
                    Functionality += decoration.delta.F;
                    Budget += decoration.cost;
                    Time += decoration.buildTime;

                    if (decoration.terraform.overlayOn.Contains(cell.data.ground.id))
                    {
                        overlay = true;
                    }
                }

                if (!hasDecoration) continue;

                var ground = cell.data.ground;
                if (overlay)
                {
                    Budget += ground.overlayCost;
                    Time += ground.overlayTime;
                }
                else
                {
                    Budget += ground.replaceCost;
                    Time += ground.replaceTime;
                }
            }
            LandscapeProjectDetailsUI.Instance.SetValues(Aesthetics, Functionality, Budget+extraCost, extraTime+Time);
        }
    }
}

