using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class BuildElementsGenerator
{
    private class ElementSpec
    {
        public int id;
        public string name;
        public Category category;
        public int cost;
        public float buildTime;
        public DeltaScore delta;
        public Tolerance tolerance;
        public float failMod;
        public AuraSpec aura;
        public List<TerrainType> terrainAllowed = new();
        public List<int> allowedBase = new();
        public List<TerrainType> overlayOn = new();
        public AdjacencySpec adjacency;
    }

    private class AuraSpec
    {
        public int radius;
        public AuraShape shape;
    }

    private class AdjacencySpec
    {
        public string needId;
        public int radius;
        public DeltaScore bonus;
    }

    [MenuItem("Tools/LD/Generate Build Elements")]
    private static void Generate()
    {
        var path = "Assets/GameData/BuildElements";
        Directory.CreateDirectory(path);

        var specs = GetSpecs();
        var map = new Dictionary<int, BuildElementData>();

        foreach (var spec in specs)
        {
            var assetPath = $"{path}/{spec.id}_{spec.name}.asset";
            var data = AssetDatabase.LoadAssetAtPath<BuildElementData>(assetPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<BuildElementData>();
                AssetDatabase.CreateAsset(data, assetPath);
            }

            data.id = spec.id;
            data.displayName = spec.name;
            data.description = BuildDescription(spec);
            data.category = spec.category;
            data.cost = spec.cost;
            data.buildTime = spec.buildTime;
            data.delta = spec.delta;

            data.stability.tolerance = spec.tolerance;
            data.stability.failMod = spec.failMod;

            data.constraints.terrainAllowed = new List<TerrainType>(spec.terrainAllowed);

            data.adjacency.Clear();
            if (spec.adjacency != null)
            {
                data.adjacency.Add(new AdjacencyRule
                {
                    needId = spec.adjacency.needId,
                    bonus = spec.adjacency.bonus
                });
            }

            data.terraform.overlayOn = new List<TerrainType>(spec.overlayOn);

            EditorUtility.SetDirty(data);
            map[spec.id] = data;
        }

        foreach (var spec in specs)
        {
            var data = map[spec.id];
            
            EditorUtility.SetDirty(data);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static string BuildDescription(ElementSpec spec)
    {
        return $"Стоимость {spec.cost}, время строительства {spec.buildTime}, функциональность {spec.delta.F} (добавляет к общей функциональности), эстетика {spec.delta.A} (добавляет к общей эстетике)";
    }

    private static List<ElementSpec> GetSpecs()
    {
        var all = new List<ElementSpec>
        {
            new ElementSpec
            {
                id = 1,
                name = "Topsoil Fill",
                category = Category.Embankment,
                cost = 8,
                buildTime = 0.2f,
                delta = new DeltaScore { F = 0, A = 0, S = 1 },
                tolerance = new Tolerance { temp = 2, wind = 0, hum = 2 },
                failMod = 0.95f,
                aura = new AuraSpec
                {
                    radius = 0,
                    shape = AuraShape.Square,
                },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                overlayOn = new List<TerrainType>()
            },
            new ElementSpec
            {
                id = 2,
                name = "Gravel-Sand Fill",
                category = Category.Embankment,
                cost = 10,
                buildTime = 0.25f,
                delta = new DeltaScore { F = 0, A = 0, S = 2 },
                tolerance = new Tolerance { temp = 2, wind = 1, hum = 1 },
                failMod = 0.9f,
                aura = new AuraSpec
                {
                    radius = 0,
                    shape = AuraShape.Square,
                },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                overlayOn = new List<TerrainType>()
            },
            new ElementSpec
            {
                id = 3,
                name = "Paver Tile",
                category = Category.Decoration,
                cost = 12,
                buildTime = 0.4f,
                delta = new DeltaScore { F = 1, A = 1, S = 1 },
                tolerance = new Tolerance(),
                failMod = 0.95f,
                aura = new AuraSpec{ radius = 0, shape = AuraShape.None },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{1,2},
                overlayOn = new List<TerrainType>()
            },
            new ElementSpec
            {
                id = 4,
                name = "Gravel Path",
                category = Category.Decoration,
                cost = 6,
                buildTime = 0.2f,
                delta = new DeltaScore { F = 1, A = 0, S = 1 },
                tolerance = new Tolerance(),
                failMod = 0.95f,
                aura = new AuraSpec{ radius = 0, shape = AuraShape.None },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{2},
                overlayOn = new List<TerrainType>{ TerrainType.Steppe, TerrainType.Swamp }
            },
            new ElementSpec
            {
                id = 5,
                name = "Wood Deck",
                category = Category.Decoration,
                cost = 18,
                buildTime = 0.6f,
                delta = new DeltaScore { F = 1, A = 1, S = 0 },
                tolerance = new Tolerance(),
                failMod = 0.95f,
                aura = new AuraSpec{ radius = 0, shape = AuraShape.None },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{2},
                overlayOn = new List<TerrainType>{ TerrainType.Swamp }
            },
            new ElementSpec
            {
                id = 6,
                name = "Asphalt",
                category = Category.Decoration,
                cost = 15,
                buildTime = 0.5f,
                delta = new DeltaScore { F = 2, A = 0, S = 1 },
                tolerance = new Tolerance(),
                failMod = 0.95f,
                aura = new AuraSpec{ radius = 0, shape = AuraShape.None },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{2},
                overlayOn = new List<TerrainType>()
            },
            new ElementSpec
            {
                id = 7,
                name = "Lawn",
                category = Category.Decoration,
                cost = 4,
                buildTime = 0.1f,
                delta = new DeltaScore { F = 0, A = 1, S = 0 },
                tolerance = new Tolerance(),
                failMod = 0.98f,
                aura = new AuraSpec{ radius = 0, shape = AuraShape.None },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{1},
                overlayOn = new List<TerrainType>()
            },
            new ElementSpec
            {
                id = 8,
                name = "Flowerbed",
                category = Category.Decoration,
                cost = 8,
                buildTime = 0.2f,
                delta = new DeltaScore { F = 0, A = 2, S = 0 },
                tolerance = new Tolerance(),
                failMod = 0.98f,
                aura = new AuraSpec{ radius = 0, shape = AuraShape.None },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{1},
                overlayOn = new List<TerrainType>()
            },
            new ElementSpec
            {
                id = 9,
                name = "Shrub",
                category = Category.Decoration,
                cost = 10,
                buildTime = 0.3f,
                delta = new DeltaScore { F = 0, A = 1, S = 1 },
                tolerance = new Tolerance(),
                failMod = 0.98f,
                aura = new AuraSpec{ radius = 0, shape = AuraShape.None },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{1},
                overlayOn = new List<TerrainType>()
            },
            new ElementSpec
            {
                id = 10,
                name = "Tree Sapling",
                category = Category.Decoration,
                cost = 12,
                buildTime = 0.3f,
                delta = new DeltaScore { F = 0, A = 1, S = 2 },
                tolerance = new Tolerance(),
                failMod = 0.98f,
                aura = new AuraSpec
                {
                    radius = 1,
                    shape = AuraShape.Circle,
                },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{1},
                overlayOn = new List<TerrainType>()
            },
            new ElementSpec
            {
                id = 11,
                name = "Pond Water",
                category = Category.Decoration,
                cost = 20,
                buildTime = 0.8f,
                delta = new DeltaScore { F = 0, A = 2, S = 0 },
                tolerance = new Tolerance(),
                failMod = 0.95f,
                aura = new AuraSpec{ radius = 0, shape = AuraShape.None },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                overlayOn = new List<TerrainType>{ TerrainType.None }
            },
            new ElementSpec
            {
                id = 12,
                name = "Fountain",
                category = Category.Decoration,
                cost = 40,
                buildTime = 1.2f,
                delta = new DeltaScore { F = 0, A = 3, S = 0 },
                tolerance = new Tolerance(),
                failMod = 0.95f,
                aura = new AuraSpec{ radius = 0, shape = AuraShape.None },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{3},
                overlayOn = new List<TerrainType>()
            },
            new ElementSpec
            {
                id = 13,
                name = "Rock Group",
                category = Category.Decoration,
                cost = 14,
                buildTime = 0.4f,
                delta = new DeltaScore { F = 0, A = 1, S = 1 },
                tolerance = new Tolerance(),
                failMod = 0.95f,
                aura = new AuraSpec{ radius = 0, shape = AuraShape.None },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{2,3},
                overlayOn = new List<TerrainType>()
            },
            new ElementSpec
            {
                id = 14,
                name = "Bench",
                category = Category.Decoration,
                cost = 6,
                buildTime = 0.1f,
                delta = new DeltaScore { F = 1, A = 0, S = 0 },
                tolerance = new Tolerance(),
                failMod = 0.95f,
                aura = new AuraSpec{ radius = 0, shape = AuraShape.None },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{3,5,6},
                overlayOn = new List<TerrainType>(),
                adjacency = new AdjacencySpec
                {
                    needId = "4",
                    radius = 1,
                    bonus = new DeltaScore { F = 1, A = 0, S = 0 }
                }
            },
            new ElementSpec
            {
                id = 15,
                name = "Bin",
                category = Category.Decoration,
                cost = 5,
                buildTime = 0.1f,
                delta = new DeltaScore { F = 1, A = 0, S = 0 },
                tolerance = new Tolerance(),
                failMod = 0.95f,
                aura = new AuraSpec{ radius = 0, shape = AuraShape.None },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{3,5,6},
                overlayOn = new List<TerrainType>()
            },
            new ElementSpec
            {
                id = 16,
                name = "Lamp",
                category = Category.Decoration,
                cost = 20,
                buildTime = 0.5f,
                delta = new DeltaScore { F = 1, A = 0, S = 1 },
                tolerance = new Tolerance(),
                failMod = 0.95f,
                aura = new AuraSpec
                {
                    radius = 2,
                    shape = AuraShape.Square,
                },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{3,5,6},
                overlayOn = new List<TerrainType>()
            },
            new ElementSpec
            {
                id = 17,
                name = "Playground",
                category = Category.Decoration,
                cost = 80,
                buildTime = 2.0f,
                delta = new DeltaScore { F = 3, A = 1, S = 1 },
                tolerance = new Tolerance(),
                failMod = 0.9f,
                aura = new AuraSpec{ radius = 0, shape = AuraShape.None },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{3,6},
                overlayOn = new List<TerrainType>()
            },
            new ElementSpec
            {
                id = 18,
                name = "Sports Ground",
                category = Category.Decoration,
                cost = 90,
                buildTime = 2.2f,
                delta = new DeltaScore { F = 3, A = 0, S = 1 },
                tolerance = new Tolerance(),
                failMod = 0.9f,
                aura = new AuraSpec{ radius = 0, shape = AuraShape.None },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{3,6},
                overlayOn = new List<TerrainType>()
            },
            new ElementSpec
            {
                id = 19,
                name = "Parking",
                category = Category.Decoration,
                cost = 60,
                buildTime = 1.5f,
                delta = new DeltaScore { F = 3, A = 0, S = 1 },
                tolerance = new Tolerance(),
                failMod = 0.9f,
                aura = new AuraSpec{ radius = 0, shape = AuraShape.None },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{6},
                overlayOn = new List<TerrainType>()
            },
            new ElementSpec
            {
                id = 20,
                name = "Gazebo",
                category = Category.Decoration,
                cost = 70,
                buildTime = 1.6f,
                delta = new DeltaScore { F = 2, A = 2, S = 1 },
                tolerance = new Tolerance(),
                failMod = 0.9f,
                aura = new AuraSpec{ radius = 0, shape = AuraShape.None },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{3,5},
                overlayOn = new List<TerrainType>()
            },
            new ElementSpec
            {
                id = 21,
                name = "BBQ Zone",
                category = Category.Decoration,
                cost = 25,
                buildTime = 0.5f,
                delta = new DeltaScore { F = 2, A = 0, S = 0 },
                tolerance = new Tolerance(),
                failMod = 0.9f,
                aura = new AuraSpec{ radius = 0, shape = AuraShape.None },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{3,6},
                overlayOn = new List<TerrainType>()
            },
            new ElementSpec
            {
                id = 22,
                name = "Small Bridge",
                category = Category.Decoration,
                cost = 50,
                buildTime = 1.4f,
                delta = new DeltaScore { F = 2, A = 1, S = 1 },
                tolerance = new Tolerance(),
                failMod = 0.9f,
                aura = new AuraSpec{ radius = 0, shape = AuraShape.None },
                terrainAllowed = new List<TerrainType>{ TerrainType.All },
                allowedBase = new List<int>{5,2},
                overlayOn = new List<TerrainType>{ TerrainType.Pound }
            }
        };

        return all;
    }
}

