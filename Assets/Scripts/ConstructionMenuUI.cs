using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class ConstructionMenuUI : MonoBehaviour
    {
        [SerializeField] private ConstructionPanel[] _constructionPanels;
        [SerializeField] private GameObject _constructionPanelPrefab;
        [SerializeField] private BuildElementUI _buildElementUI;
        
        private void Start()
        {
            foreach (var constructionPanel in _constructionPanels)
            {
                var panel = Instantiate(_constructionPanelPrefab, transform, false);
                panel.GetComponentInChildren<TMP_Text>().text = constructionPanel.title;
                foreach (var buildElementData in constructionPanel.buildElementDatas)
                {
                    var buildElementUI = Instantiate(_buildElementUI, panel.transform, false);
                    buildElementUI.SetData(buildElementData);
                }
            }
        }
    }

    [Serializable]
    public class ConstructionPanel
    {
        public BuildElementData[] buildElementDatas;
        public string title;
    }
}