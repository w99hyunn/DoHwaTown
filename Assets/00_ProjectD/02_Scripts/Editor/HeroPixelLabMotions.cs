using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace DoHwa.Editor
{
    public static class HeroPixelLabMotions
    {
        [MenuItem("DoHwa/Apply PixelLab Motions")]
        public static void Apply()
        {
            string root = "Assets/00_DoHwaTown";
            string[] directions = { "South", "SouthEast", "East", "NorthEast", "North", "NorthWest", "West", "SouthWest" };
            string[] sources = { "South", "SouthEast", "East", "NorthEast", "North", "NorthEast", "East", "SouthEast" };
            string[] motions = { "Idle", "Walk", "Run", "Dash" };
            // Check all expected files before changing the currently referenced assets.
            foreach (string direction in sources.Distinct())
                foreach (string motion in motions)
                {
                    int count = motion == "Walk" || motion == "Run" ? 9 : 5;
                    for (int i = 0; i < count; i++)
                    {
                        string file = $"{root}/06_Model/Characters/Hero/Textures/PixelLab/{direction}/{motion}/{i:00}.png";
                        if (!File.Exists(file)) throw new FileNotFoundException("Missing PixelLab frame", file);
                    }
                }
            Directory.CreateDirectory(root + "/06_Model/Characters/Hero/Sprites/PixelLab");
            Directory.CreateDirectory(root + "/07_Animation/PixelLab");
            AssetDatabase.Refresh();
            string controllerPath = root + "/07_Animation/PixelLab.controller";
            var controller = File.Exists(controllerPath) ? AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath)
                : AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            var machine = controller.layers[0].stateMachine;
            for (int direction = 0; direction < directions.Length; direction++)
            {
                var reference = Load($"{root}/06_Model/Characters/Hero/Textures/PixelLab/{sources[direction]}/Idle/00.png");
                var colors = reference.GetPixels32();
                int bottom = reference.height, top = 0;
                for (int y = 0; y < reference.height; y++)
                    for (int x = 0; x < reference.width; x++)
                        if (colors[y * reference.width + x].a > 128) { bottom = Math.Min(bottom, y); top = Math.Max(top, y); }
                float ppu = (top - bottom + 1) / 1.7f;
                for (int motion = 0; motion < motions.Length; motion++)
                {
                    string name = directions[direction] + motions[motion];
                    int count = motion == 1 || motion == 2 ? 9 : 5;
                    var frames = new Sprite[count];
                    for (int i = 0; i < count; i++)
                    {
                        var texture = Load($"{root}/06_Model/Characters/Hero/Textures/PixelLab/{sources[direction]}/{motions[motion]}/{i:00}.png");
                        if (texture.width != reference.width || texture.height != reference.height)
                            throw new InvalidOperationException(name + " frame size changed");
                        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                            new Vector2(0.5f, bottom / (float)texture.height), ppu, 0, SpriteMeshType.FullRect);
                        sprite.name = name + i.ToString("00");
                        string spritePath = root + "/06_Model/Characters/Hero/Sprites/PixelLab/" + sprite.name + ".asset";
                        if (File.Exists(spritePath))
                        {
                            frames[i] = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                            EditorUtility.CopySerialized(sprite, frames[i]);
                            EditorUtility.SetDirty(frames[i]);
                            UnityEngine.Object.DestroyImmediate(sprite);
                        }
                        else { AssetDatabase.CreateAsset(sprite, spritePath); frames[i] = sprite; }
                    }
                    string clipPath = root + "/07_Animation/PixelLab/" + name + ".anim";
                    var clip = File.Exists(clipPath) ? AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath) : new AnimationClip { name = name };
                    if (!File.Exists(clipPath)) AssetDatabase.CreateAsset(clip, clipPath);
                    float fps = new[] { 4f, 12f, 18f, 20f }[motion];
                    var keys = new ObjectReferenceKeyframe[count];
                    for (int i = 0; i < count; i++)
                        keys[i] = new ObjectReferenceKeyframe { time = i / fps, value = frames[i] };
                    clip.frameRate = fps;
                    AnimationUtility.SetObjectReferenceCurve(clip, new EditorCurveBinding { path = "", type = typeof(SpriteRenderer), propertyName = "m_Sprite" }, keys);
                    var settings = AnimationUtility.GetAnimationClipSettings(clip);
                    settings.loopTime = motion != 3;
                    AnimationUtility.SetAnimationClipSettings(clip, settings);
                    EditorUtility.SetDirty(clip);
                    var state = machine.states.FirstOrDefault(s => s.state.name == name).state;
                    if (!state) state = machine.AddState(name, new Vector3(motion * 230, direction * 65));
                    state.motion = clip;
                    if (direction == 0 && motion == 0) machine.defaultState = state;
                }
            }
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
        }

        private static Texture2D Load(string path)
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            if (!importer.isReadable || importer.filterMode != FilterMode.Point || importer.mipmapEnabled
                || importer.textureCompression != TextureImporterCompression.Uncompressed || importer.npotScale != TextureImporterNPOTScale.None)
            {
                importer.isReadable = true;
                importer.filterMode = FilterMode.Point;
                importer.mipmapEnabled = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
    }
}


