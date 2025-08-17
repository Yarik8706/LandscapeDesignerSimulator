// Assets/Editor/BuildElementPrefabGenerator.cs
using System.IO;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;

public static class BuildElementPrefabGenerator
{
    private const string SourceFolder = "Assets/GameData/BuildElements";
    private const string OutputRoot   = "Assets/Prefabs";
    private const string OutputFolder = "Assets/Prefabs/BuildElements";

    [MenuItem("Tools/Build/Generate BuildElement Prefabs")]
    public static void Generate()
    {
        // Проверки папок
        EnsureFolder("Assets", "Prefabs");
        EnsureFolder(OutputRoot, "BuildElements");

        var guids = AssetDatabase.FindAssets("t:BuildElementData", new[] { SourceFolder });
        if (guids.Length == 0)
        {
            Debug.LogWarning($"[BuildElementPrefabGenerator] Не найдено ни одного BuildElementData в {SourceFolder}");
            return; 
        }

        int created = 0, updated = 0, skipped = 0;
        try
        {
            for (int i = 0; i < guids.Length; i++)
            {
                string soPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var data = AssetDatabase.LoadAssetAtPath<BuildElementData>(soPath);
                if (data == null)
                {
                    skipped++;
                    continue;
                }

                // Имя префаба = имя файла SO
                string name = Path.GetFileNameWithoutExtension(soPath);
                string prefabPath = $"{OutputFolder}/{name}.prefab";

                // Достаём sprite из поля icon
                Sprite icon = data.icon;
                if (icon == null)
                {
                    Debug.LogWarning($"[BuildElementPrefabGenerator] {name}: поле 'icon' пустое — пропуск.");
                    skipped++;
                    continue;
                }

                EditorUtility.DisplayProgressBar("Генерация префабов", name, (float)i / guids.Length);

                var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (existingPrefab == null)
                {
                    // Создаём новый
                    var go = new GameObject(name);
                    var sr = go.AddComponent<SpriteRenderer>();
                    sr.sprite = icon;
                    var be = go.AddComponent<BuildElement>();
                    be.BuildElementData = data;

                    PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                    Object.DestroyImmediate(go);
                    created++;
                    
                    var prefabAssetReloaded = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    data.prefab = prefabAssetReloaded.GetComponent<BuildElement>();
                    EditorUtility.SetDirty(data);
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        Debug.Log($"[BuildElementPrefabGenerator] Готово. Создано: {created}, обновлено: {updated}, пропущено: {skipped}.");
    }

    private static void EnsureFolder(string parent, string child)
    {
        string path = $"{parent}/{child}";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder(parent, child);
        }
    }
    
    private static BuildElement GetSpriteRendererOnAsset(GameObject prefabAsset, string path = "")
    {
        if (string.IsNullOrEmpty(path))
            return prefabAsset.GetComponent<BuildElement>();

        // Идём по пути в активе
        var t = prefabAsset.transform;
        foreach (var part in path.Split('/'))
        {
            var next = t.Find(part);
            if (next == null) return null;
            t = next;
        }
        return t.GetComponent<BuildElement>();
    }
}
