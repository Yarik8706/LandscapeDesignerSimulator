using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class ConstructionMenuUI : MonoBehaviour
    {
        [SerializeField] private ConstructionPanelData[] _constructionPanels;
        [SerializeField] private ConstructionPanelUI _constructionPanelPrefab;
        [SerializeField] private BuildElementUI _buildElementUI;
        
        [SerializeField] private Transform _constructionPanelContainer;
        
        private void Start()
        {
            foreach (var constructionPanel in _constructionPanels)
            {
                var panel = Instantiate(_constructionPanelPrefab, 
                    _constructionPanelContainer);
                panel.title.text = constructionPanel.title;
                foreach (var buildElementData in constructionPanel.buildElementDatas)
                {
                    var buildElementUI = Instantiate(_buildElementUI, panel.elementsContainer, false);
                    buildElementUI.SetData(buildElementData);
                }
            }
        }
    }

    [Serializable]
    public class ConstructionPanelData
    {
        public BuildElementData[] buildElementDatas;
        public string title;
    }
}