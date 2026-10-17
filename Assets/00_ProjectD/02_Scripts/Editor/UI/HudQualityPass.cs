using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using DoHwa.UI;
namespace DoHwa.Editor {
public static class HudQualityPass {
static string dir="Assets/00_ProjectD/04_UI/Modern/";
static T C<T>(Transform t) where T:Component {t.TryGetComponent<T>(out var c);return c;}
static void P(Transform t,float x,float y,float w,float h){var r=(RectTransform)t;r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);}
static Sprite S(string n)=>AssetDatabase.LoadAssetAtPath<Sprite>(dir+n+".asset");
static void Txt(Transform t,float size,Color color){var text=C<TMP_Text>(t);text.fontSize=size;text.color=color;text.textWrappingMode=TextWrappingModes.NoWrap;}
static void Frame(Transform t,string n){var im=C<UnityEngine.UI.Image>(t);im.sprite=S(n);im.type=UnityEngine.UI.Image.Type.Simple;im.color=Color.white;}
static void Slice(Texture2D tex,string n,Rect rect){if(S(n))return;var s=Sprite.Create(tex,new Rect(rect.x,tex.height-rect.y-rect.height,rect.width,rect.height),new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect);s.name=n;AssetDatabase.CreateAsset(s,dir+n+".asset");}
public static void RepairAtlas(){
foreach(string atlas in new[]{"Frames","Icons"}){var path=dir+atlas+".png";var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=2048;importer.SaveAndReimport();}
var tex=AssetDatabase.LoadAssetAtPath<Texture2D>(dir+"Frames.png");
string[] names={"SlotNormal","SlotSelected","DetailPaper","ButtonPrimary","ButtonPaper","MinimapRing","GaugeFrame","StartBand","TabSelected"};
Rect[] rects={new Rect(28,74,365,445),new Rect(450,74,360,445),new Rect(868,75,365,445),new Rect(20,690,405,110),new Rect(462,690,410,110),new Rect(910,575,320,310),new Rect(24,1050,425,55),new Rect(465,1020,490,120),new Rect(1015,984,195,192)};
for(int i=0;i<names.Length;i++)UpdateSprite(tex,names[i],rects[i]);
tex=AssetDatabase.LoadAssetAtPath<Texture2D>(dir+"Icons.png");names=new[]{"Bag","Craft","Journal","Settings","Equipment","Food","Material","Map","Language","Notice","Exit","Close"};
for(int i=0;i<12;i++)UpdateSprite(tex,"Icon"+names[i],new Rect(i%4*320,new[]{225,525,840}[i/4],Mathf.Min(320,tex.width-i%4*320),265));AssetDatabase.SaveAssets();}
static void UpdateSprite(Texture2D tex,string name,Rect rect){var sprite=Sprite.Create(tex,new Rect(rect.x,tex.height-rect.y-rect.height,rect.width,rect.height),new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect);sprite.name=name;var existing=S(name);EditorUtility.CopySerialized(sprite,existing);Object.DestroyImmediate(sprite);EditorUtility.SetDirty(existing);}
public static void Import(){AssetDatabase.Refresh();var path=dir+"HudSurfaces.png";var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Default;importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=2048;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.SaveAndReimport();var tex=AssetDatabase.LoadAssetAtPath<Texture2D>(path);
Slice(tex,"QuickNormal",new Rect(235,167,397,393));Slice(tex,"QuickSelected",new Rect(905,166,400,397));Slice(tex,"QuestScrim",new Rect(48,680,680,218));Slice(tex,"VitalsSurface",new Rect(788,727,704,140));AssetDatabase.SaveAssets();}
public static void Apply(){var scene=EditorSceneManager.OpenScene("Assets/00_ProjectD/01_Scenes/Production/Ingame.unity");var root=GameObject.Find("RpgCanvas").transform;var hud=root.Find("HUD");var controller=C<RpgUiController>(root);
var region=hud.Find("Region");P(region,36,32,410,500);var map=region.Find("MapFrame");P(map,0,0,204,204);P(map.Find("Mask"),4,4,196,196);P(map.Find("Mask/Terrain"),0,0,196,196);P(map.Find("IllustratedRing"),-5,-5,214,214);P(map.Find("North"),87,-8,30,25);P(map.Find("Marker"),90,90,24,24);
P(region.Find("Name"),0,214,220,32);Txt(region.Find("Name"),24,Color.white);P(region.Find("Season"),0,247,220,24);Txt(region.Find("Season"),16,new Color(.88f,.90f,.94f));
var obj=region.Find("Objective");P(obj,-12,290,390,132);Frame(obj,"QuestScrim");P(obj.Find("Tag"),26,10,340,24);Txt(obj.Find("Tag"),15,new Color(.81f,.78f,1));P(obj.Find("Title"),26,38,340,32);Txt(obj.Find("Title"),22,Color.white);P(obj.Find("Hint"),26,79,340,26);Txt(obj.Find("Hint"),16,new Color(.93f,.94f,.96f));C<TMP_Text>(obj.Find("Title")).text="숲으로 떠날 준비";C<TMP_Text>(obj.Find("Hint")).text="배낭에서 도구와 식량 확인   B";
C<UnityEngine.UI.Image>(obj).raycastTarget=true;if(!obj.TryGetComponent<UnityEngine.UI.Button>(out var objectiveButton)){objectiveButton=obj.gameObject.AddComponent<UnityEngine.UI.Button>();UnityEventTools.AddPersistentListener(objectiveButton.onClick,controller.OpenInventory);}objectiveButton.targetGraphic=C<UnityEngine.UI.Image>(obj);
var nav=hud.Find("Navigation");((RectTransform)nav).anchoredPosition=new Vector2(-390,-36);((RectTransform)nav).sizeDelta=new Vector2(350,90);int n=0;foreach(string name in new[]{"Bag","Craft","Journal","Settings"}){var t=nav.Find(name);P(t,n++*84,0,70,76);P(t.Find("Glyph"),15,0,40,40);P(t.Find("Label"),0,48,70,22);Txt(t.Find("Label"),15,Color.white);}
var quick=hud.Find("Quickbar");var qr=(RectTransform)quick;qr.anchoredPosition=new Vector2(-300,110);qr.sizeDelta=new Vector2(600,70);int index=0;foreach(Transform t in quick){P(t,index++*76,0,68,68);C<RpgItemButton>(t).ConfigureSkin(S("QuickNormal"),S("QuickSelected"));Frame(t,"QuickNormal");P(t.Find("Art"),7,6,54,54);P(t.Find("Key"),6,3,18,18);Txt(t.Find("Key"),12,new Color(.87f,.89f,.93f));P(t.Find("Count"),34,45,27,20);Txt(t.Find("Count"),17,Color.white);C<TMP_Text>(t.Find("Count")).alignment=TextAlignmentOptions.Right;}
var vital=hud.Find("Vitals");var vr=(RectTransform)vital;vr.anchoredPosition=new Vector2(-220,178);vr.sizeDelta=new Vector2(440,56);Frame(vital,"VitalsSurface");P(vital.Find("HealthText"),14,0,412,24);Txt(vital.Find("HealthText"),17,Color.white);P(vital.Find("HealthTrack"),20,27,400,6);P(vital.Find("FoodText"),280,40,140,23);Txt(vital.Find("FoodText"),14,Color.white);C<TMP_Text>(vital.Find("FoodText")).alignment=TextAlignmentOptions.Right;P(vital.Find("FoodTrack"),20,47,245,3);
foreach(string name in new[]{"HealthTrack","FoodTrack"}){var track=vital.Find(name);track.Find("IllustratedFrame").gameObject.SetActive(false);var tr=C<UnityEngine.UI.Image>(track);tr.color=new Color(.12f,.14f,.18f,.9f);var fill=(RectTransform)track.Find("Fill");fill.anchorMin=Vector2.zero;fill.anchorMax=Vector2.one;fill.offsetMin=fill.offsetMax=Vector2.zero;var im=C<UnityEngine.UI.Image>(fill);im.type=UnityEngine.UI.Image.Type.Filled;im.fillMethod=UnityEngine.UI.Image.FillMethod.Horizontal;im.fillOrigin=0;im.color=name=="HealthTrack"?new Color(.65f,.85f,.69f):new Color(.73f,.68f,.89f);}
// Exclude foliage from the dedicated terrain camera, without changing main camera visibility.
var tags=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);var layers=tags.FindProperty("layers");if(layers.GetArrayElementAtIndex(30).stringValue!="" && layers.GetArrayElementAtIndex(30).stringValue!="MinimapHidden")throw new System.InvalidOperationException("Layer 30 already used");layers.GetArrayElementAtIndex(30).stringValue="MinimapHidden";tags.ApplyModifiedProperties();
foreach(string group in new[]{"Trees","Understory","Fallen Leaf Carpet","Atmosphere"})foreach(var t in GameObject.Find("Peach Forest").transform.Find(group).GetComponentsInChildren<Transform>(true))t.gameObject.layer=30;
foreach(var t in GameObject.Find("Windblown Blossom Petals").GetComponentsInChildren<Transform>(true))t.gameObject.layer=30;
GameObject.Find("UI Minimap Camera").TryGetComponent<Camera>(out var camera);camera.cullingMask&=~(1<<30);camera.backgroundColor=new Color(.24f,.29f,.29f);
controller.Bind();EditorSceneManager.SaveScene(scene);}
}}

