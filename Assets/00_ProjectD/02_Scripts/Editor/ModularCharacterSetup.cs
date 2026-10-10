using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace DoHwa.Editor
{
    public static class ModularCharacterSetup
    {
        [MenuItem("DoHwa/Create Modular Character Prefab")]
        public static void Create()
        {
            string root = "Assets/00_DoHwaTown/06_Model/Characters/Hero/Modular/";
            var reference = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/00_DoHwaTown/06_Model/Characters/Hero/Sprites/PixelLab/SouthIdle00.asset");
            var material = AssetDatabase.LoadAssetAtPath<Material>("Assets/00_DoHwaTown/05_Materials/HeroSprite.mat");
            string[] parts = { "BackHair", "Body", "Boots", "Bottom", "Top", "SleeveLeft", "SleeveRight", "FrontHair", "Ornaments" };
            var facings = (AvatarFacing[])System.Enum.GetValues(typeof(AvatarFacing));
            int[][] orders = {
                new[] {0,1,2,3,4,5,6,7,8}, new[] {0,2,3,4,5,6,1,7,8},
                new[] {0,2,3,4,5,6,1,7,8}, new[] {7,0,1,2,3,5,4,6,8},
                new[] {7,0,1,2,3,4,5,6,8}, new[] {7,0,1,2,3,4,5,6,8},
                new[] {0,2,3,4,5,1,6,7,8}, new[] {0,2,3,4,5,1,6,7,8}
            };
            foreach (var facing in facings)
            foreach (string part in parts)
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(root + facing + "/" + part + ".png");
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = reference.pixelsPerUnit;
                importer.filterMode = FilterMode.Point;
                importer.mipmapEnabled = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.alphaIsTransparency = true;
                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteAlignment = (int)SpriteAlignment.Custom;
                settings.spritePivot = reference.pivot / reference.rect.size;
                settings.spriteMeshType = SpriteMeshType.FullRect;
                importer.SetTextureSettings(settings);
                importer.SaveAndReimport();
            }

            var rig = new GameObject("ModularHero");
            try
            {
                rig.AddComponent<SortingGroup>();
                var renderers = new Dictionary<string, SpriteRenderer>();
                for (int i = 0; i < parts.Length; i++)
                {
                    var part = new GameObject(parts[i]);
                    part.transform.SetParent(rig.transform, false);
                    var renderer = part.AddComponent<SpriteRenderer>();
                    renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(root + "South/" + parts[i] + ".png");
                    renderer.sharedMaterial = material;
                    renderer.sortingOrder = i;
                    renderers.Add(parts[i], renderer);
                }
                var character = rig.AddComponent<ModularCharacter>();
                var serialized = new SerializedObject(character);
                var slots = serialized.FindProperty("slots");
                slots.arraySize = 5;
                void Slot(int index, AvatarSlot kind, params string[] layers)
                {
                    var slot = slots.GetArrayElementAtIndex(index);
                    slot.FindPropertyRelative("slot").enumValueIndex = (int)kind;
                    var targets = slot.FindPropertyRelative("renderers");
                    targets.arraySize = layers.Length;
                    for (int i = 0; i < layers.Length; i++)
                        targets.GetArrayElementAtIndex(i).objectReferenceValue = renderers[layers[i]];
                }
                Slot(0, AvatarSlot.Hair, "BackHair", "FrontHair");
                Slot(1, AvatarSlot.Top, "Top", "SleeveLeft", "SleeveRight");
                Slot(2, AvatarSlot.Bottom, "Bottom");
                Slot(3, AvatarSlot.Boots, "Boots");
                Slot(4, AvatarSlot.Ornaments, "Ornaments");
                var allLayers = serialized.FindProperty("layers");
                allLayers.arraySize = parts.Length;
                for (int i = 0; i < parts.Length; i++)
                    allLayers.GetArrayElementAtIndex(i).objectReferenceValue = renderers[parts[i]];
                var frames = serialized.FindProperty("directions");
                frames.arraySize = facings.Length;
                for (int d = 0; d < facings.Length; d++)
                {
                    var frame = frames.GetArrayElementAtIndex(d);
                    frame.FindPropertyRelative("direction").enumValueIndex = (int)facings[d];
                    var sprites = frame.FindPropertyRelative("sprites");
                    var sorting = frame.FindPropertyRelative("sortingOrders");
                    sprites.arraySize = sorting.arraySize = parts.Length;
                    for (int i = 0; i < parts.Length; i++)
                    {
                        sprites.GetArrayElementAtIndex(i).objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>(root + facings[d] + "/" + parts[i] + ".png");
                        sorting.GetArrayElementAtIndex(i).intValue = orders[d][i];
                    }
                }
                serialized.ApplyModifiedPropertiesWithoutUndo();
                AssignMotions(character);
                PrefabUtility.SaveAsPrefabAsset(rig, "Assets/00_DoHwaTown/03_Prefabs/B_Characters/ModularHero.prefab");
                AssetDatabase.SaveAssets();
            }
            finally { Object.DestroyImmediate(rig); }
        }
        public static void AssignMotions(ModularCharacter character)
        {
            string root = "Assets/00_DoHwaTown/06_Model/Characters/Hero/Modular/Motions/";
            string[] parts = { "BackHair", "Body", "Boots", "Bottom", "Top", "SleeveLeft", "SleeveRight", "FrontHair", "Ornaments" };
            var serialized = new SerializedObject(character);
            var clips = serialized.FindProperty("motions");
            clips.arraySize = 24;
            for (int direction = 0; direction < 8; direction++)
                for (int motion = 1; motion <= 3; motion++)
                {
                    var clip = clips.GetArrayElementAtIndex(direction * 3 + motion - 1);
                    clip.FindPropertyRelative("direction").enumValueIndex = direction;
                    clip.FindPropertyRelative("motion").enumValueIndex = motion;
                    clip.FindPropertyRelative("framesPerSecond").floatValue = motion == 1 ? 10f : motion == 2 ? 12.5f : 25f;
                    var sprites = clip.FindPropertyRelative("sprites");
                    sprites.arraySize = 8 * parts.Length;
                    for (int part = 0; part < parts.Length; part++)
                    {
                        string path = root + (AvatarFacing)direction + "/" + (AvatarMotion)motion + "/" + parts[part] + ".png";
                        var frames = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(sprite => sprite.name).ToArray();
                        if (frames.Length != 8) throw new System.InvalidOperationException("Expected 8 motion frames: " + path);
                        for (int frame = 0; frame < 8; frame++)
                        {
                            // Walk source sheets lift the foot during the backward sweep; reverse the cycle for forward travel.
                            int sourceFrame = motion == (int)AvatarMotion.Walk ? (frames.Length - frame) % frames.Length : frame;
                            sprites.GetArrayElementAtIndex(frame * parts.Length + part).objectReferenceValue = frames[sourceFrame];
                        }
                    }
                }
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
