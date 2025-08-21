using UnityEditor;
using UnityEngine;
using System.IO;

public class FixBuildElementDescriptions : EditorWindow
{
    [MenuItem("Tools/Fix Build Element Descriptions")]
    public static void FixDescriptions()
    {
        string[] guids = AssetDatabase.FindAssets("t:BuildElementData", new[] { "Assets/GameData/BuildElements" });
        int fixedCount = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            BuildElementData data = AssetDatabase.LoadAssetAtPath<BuildElementData>(path);
            
            if (data != null && !string.IsNullOrEmpty(data.description))
            {
                string originalDescription = data.description;
                string newDescription = data.description.Replace("/n", "\n");
                
                if (originalDescription != newDescription)
                {
                    data.description = newDescription;
                    EditorUtility.SetDirty(data);
                    fixedCount++;
                    Debug.Log($"Fixed description in {data.name}");
                }
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Fixed descriptions in {fixedCount} BuildElementData assets.");
    }
}
