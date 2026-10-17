using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DoHwa.UI
{
    public sealed class RpgUiController : MonoBehaviour
    {
        [Serializable]
        public class Item
        {
            public string title, category, description;
            public Sprite icon;
            public int quantity;
        }

        [SerializeField] private Item[] items;
        [SerializeField] private HeroMotor motor;
        [SerializeField] private HeroCamera view;
        [SerializeField] private Transform hero;
        [SerializeField] private Camera minimapCamera;
        private GameObject inventory, map, journal, settings;
        private RectTransform minimapMarker;
        private TMP_Text title, category, description, count, actionLabel, capacity, healthLabel, foodLabel, feedback;
        private UnityEngine.UI.Image detailArt;
        private UiGaugeMotion healthMotion, foodMotion;
        private UnityEngine.UI.Button action;
        private RpgItemButton[] slots, quickSlots;
        private int selected, filter, equipped, quickSelected;
        private bool crafting, wasMotorEnabled, wasViewEnabled, modal, reverseSort;
        private float health = 80, food = 75, feedbackUntil;
        private readonly int[] quickItems = { 0, 1, 2, 3, 4, 6, 7, 9 };
        private InputAction bagKey, craftKey, mapKey, journalKey, escapeKey;
        private InputAction[] quickKeys;
        private int transition;

        public void Configure(Item[] catalog, HeroMotor movement, HeroCamera cameraView, Camera mini)
        {
            items = catalog; motor = movement; view = cameraView; hero = movement.transform; minimapCamera = mini;
        }

        private T Component<T>(string path) where T : Component
        {
            transform.Find(path).TryGetComponent<T>(out var component);
            return component;
        }

        public void Bind()
        {
            inventory = transform.Find("Inventory").gameObject;
            map = transform.Find("Map").gameObject;
            journal = transform.Find("Journal").gameObject;
            settings = transform.Find("Settings").gameObject;
            minimapMarker = (RectTransform)transform.Find("HUD/Region/MapFrame/Marker");
            title=Component<TMP_Text>("Inventory/Detail/Title");
            category=Component<TMP_Text>("Inventory/Detail/Category");
            description=Component<TMP_Text>("Inventory/Detail/Description");
            count=Component<TMP_Text>("Inventory/Detail/Count");
            actionLabel=Component<TMP_Text>("Inventory/Detail/Action/Label");
            action=Component<UnityEngine.UI.Button>("Inventory/Detail/Action");
            detailArt=Component<UnityEngine.UI.Image>("Inventory/Detail/Art");
            capacity=Component<TMP_Text>("Inventory/Capacity");
            healthLabel=Component<TMP_Text>("HUD/Vitals/HealthText");
            foodLabel=Component<TMP_Text>("HUD/Vitals/FoodText");
            healthMotion=Component<UiGaugeMotion>("HUD/Vitals/HealthTrack/Fill");
            foodMotion=Component<UiGaugeMotion>("HUD/Vitals/FoodTrack/Fill");
            feedback=Component<TMP_Text>("Feedback/Label");
            slots=transform.Find("Inventory/Grid").GetComponentsInChildren<RpgItemButton>(true);
            quickSlots=transform.Find("HUD/Quickbar").GetComponentsInChildren<RpgItemButton>(true);
            Refresh();
        }

        private void Awake()
        {
            Bind();
            bagKey=new InputAction("Bag",InputActionType.Button,"<Keyboard>/b");
            craftKey=new InputAction("Craft",InputActionType.Button,"<Keyboard>/c");
            mapKey=new InputAction("Map",InputActionType.Button,"<Keyboard>/m");
            journalKey=new InputAction("Journal",InputActionType.Button,"<Keyboard>/j");
            escapeKey=new InputAction("Close",InputActionType.Button,"<Keyboard>/escape");
            quickKeys=Enumerable.Range(1,8).Select(i=>new InputAction("Quick"+i,InputActionType.Button,"<Keyboard>/"+i)).ToArray();
        }

        private void OnEnable()
        {
            bagKey.Enable(); craftKey.Enable(); mapKey.Enable(); journalKey.Enable(); escapeKey.Enable();
            foreach(var key in quickKeys) key.Enable();
        }

        private void OnDisable()
        {
            bagKey.Disable(); craftKey.Disable(); mapKey.Disable(); journalKey.Disable(); escapeKey.Disable();
            foreach(var key in quickKeys) key.Disable();
            if(modal) RestoreMovement();
        }

        private void OnDestroy()
        {
            bagKey.Dispose(); craftKey.Dispose(); mapKey.Dispose(); journalKey.Dispose(); escapeKey.Dispose();
            foreach(var key in quickKeys) key.Dispose();
        }

        private void Update()
        {
            if(bagKey.WasPressedThisFrame()) { if(inventory.activeSelf && !crafting) Close(); else OpenInventory(); }
            if(craftKey.WasPressedThisFrame()) { if(inventory.activeSelf && crafting) Close(); else OpenCrafting(); }
            if(mapKey.WasPressedThisFrame()) { if(map.activeSelf) Close(); else OpenMap(); }
            if(journalKey.WasPressedThisFrame()) { if(journal.activeSelf) Close(); else OpenJournal(); }
            if(escapeKey.WasPressedThisFrame()) { if(modal) Close(); else OpenSettings(); }
            if(!modal) for(int i=0;i<quickKeys.Length;i++) if(quickKeys[i].WasPressedThisFrame()) UseQuick(i);
            if(feedback.transform.parent.gameObject.activeSelf && Time.unscaledTime>feedbackUntil) feedback.transform.parent.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            minimapCamera.transform.position=hero.position+Vector3.up*(map.activeSelf?90:45);
            minimapCamera.orthographicSize=map.activeSelf?35:14;
            float headingX=Vector3.Dot(motor.Facing,minimapCamera.transform.right);
            float headingY=Vector3.Dot(motor.Facing,minimapCamera.transform.up);
            minimapMarker.localRotation=Quaternion.Euler(0,0,-Mathf.Atan2(headingX,headingY)*Mathf.Rad2Deg);
        }

        private void Show(GameObject panel)
        {
            transition++;
            bool alreadyOpen = panel.activeSelf;
            HideQuickTooltip();
            if(!modal) { wasMotorEnabled=motor.enabled; wasViewEnabled=view.enabled; motor.enabled=false; view.enabled=false; modal=true; }
            inventory.SetActive(panel==inventory); map.SetActive(panel==map); journal.SetActive(panel==journal); settings.SetActive(panel==settings);
            transform.Find("HUD").gameObject.SetActive(false);
            if(alreadyOpen && panel.TryGetComponent<UiPanelReveal>(out var reveal)) reveal.Replay();
        }

        private void RestoreMovement()
        {
            // Scene teardown may destroy the character before the UI is disabled.
            if(motor) motor.enabled=wasMotorEnabled;
            if(view) view.enabled=wasViewEnabled;
            modal=false;
        }
        public async void Close()
        {
            int current = ++transition;
            GameObject panel = inventory.activeSelf ? inventory : map.activeSelf ? map : journal.activeSelf ? journal : settings;
            if(panel.activeSelf && panel.TryGetComponent<UiPanelReveal>(out var reveal)) await reveal.HideAsync();
            if(!this || current != transition) return;
            inventory.SetActive(false); map.SetActive(false); journal.SetActive(false); settings.SetActive(false);
            transform.Find("HUD").gameObject.SetActive(true);
            if(modal) RestoreMovement();
        }

        public void OpenInventory() { crafting=false; filter=0; Show(inventory); Select(0); }
        public void OpenCrafting() { crafting=true; filter=0; Show(inventory); Select(3); }
        public void OpenMap() => Show(map);
        public void OpenJournal() => Show(journal);
        public void OpenSettings()
        {
            Component<UnityEngine.UI.Slider>("Settings/Panel/Volume").SetValueWithoutNotify(AudioListener.volume);
            Show(settings);
        }
        public void SetVolume(float value) => AudioListener.volume=value;
        public void SetFilter(int value) { filter=value; crafting=false; Refresh(); }
        public void Select(int index) { selected=index; Refresh(); }
        public void ShowQuickTooltip(int index)
        {
            var item=items[quickItems[index]];
            var tip=transform.Find("HUD/QuickTooltip");
            tip.gameObject.SetActive(true);
            Component<TMP_Text>("HUD/QuickTooltip/Label").text=item.title+"   /   "+item.quantity+"개";
        }
        public void HideQuickTooltip() => transform.Find("HUD/QuickTooltip").gameObject.SetActive(false);
        public void Sort() { reverseSort=!reverseSort; Refresh(); Notify(reverseSort?"수량이 많은 순으로 정렬했습니다.":"기본 순서로 정렬했습니다."); }

        public void UseQuick(int index)
        {
            crafting=false;
            quickSelected=index; selected=quickItems[index];
            if(selected==2 || selected==3 || selected==7) UseSelected();
            else { if(selected==0 || selected==1) equipped=selected; Notify(items[selected].title+" 선택"); }
            Refresh();
        }

        public void UseSelected()
        {
            if(crafting)
            {
                if(selected==3 && items[6].quantity>=3 && items[2].quantity>=1) { items[6].quantity-=3; items[2].quantity--; items[3].quantity++; Notify("회복약 1개를 만들었습니다."); }
                else if(selected==11 && items[5].quantity>=3) { items[5].quantity-=3; items[11].quantity++; Notify("금속 주괴 1개를 만들었습니다."); }
                else Notify("제작에 필요한 재료가 부족합니다.");
            }
            else if(items[selected].quantity>0)
            {
                if(selected==2 || selected==7) { if(food>=100) { Notify("이미 배가 부릅니다."); return; } items[selected].quantity--; food=Mathf.Min(100,food+20); health=Mathf.Min(100,health+5); Notify(items[selected].title+" 사용 / 포만감 +20"); }
                else if(selected==3) { if(health>=100) { Notify("체력이 가득 찼습니다."); return; } items[3].quantity--; health=Mathf.Min(100,health+35); Notify("체력을 회복했습니다."); }
                else if(selected==0 || selected==1 || selected==10) { equipped=selected; Notify(items[selected].title+" 장착"); }
            }
            Refresh();
        }

        public void Notify(string message)
        {
            feedback.text=message; feedback.transform.parent.gameObject.SetActive(true); feedbackUntil=Time.unscaledTime+2.8f;
        }

        private bool Visible(int index)
        {
            if(crafting) return index==3 || index==11;
            if(filter==0) return true;
            return filter==1 ? items[index].category=="장비" : filter==2 ? items[index].category=="소모품" : items[index].category=="재료";
        }

        public void Refresh()
        {
            var visible=Enumerable.Range(0,items.Length).Where(Visible).ToArray();
            if(reverseSort) visible=visible.OrderByDescending(i=>items[i].quantity).ToArray();
            if(!visible.Contains(selected)) selected=visible[0];
            for(int i=0;i<slots.Length;i++)
            {
                int id=i<visible.Length?visible[i]:-1;
                slots[i].Show(id,id>=0?items[id].icon:null,id>=0?items[id].quantity:0,id==selected,id==equipped,!crafting&&filter==0);
                slots[i].transform.SetSiblingIndex(i);
            }
            for(int i=0;i<quickSlots.Length;i++) { int id=quickItems[i]; quickSlots[i].Show(i,items[id].icon,items[id].quantity,i==quickSelected,false); }
            var item=items[selected]; title.text=item.title; category.text=crafting?"제작법  /  손으로 만들기":item.category+"  /  도화림";
            detailArt.sprite=item.icon; detailArt.preserveAspect=true;
            description.text=item.description;
            count.text=crafting ? selected==3?$"필요 재료\n\n약초<pos=240>{items[6].quantity} / 3\n복숭아<pos=240>{items[2].quantity} / 1":$"필요 재료\n\n철광석<pos=240>{items[5].quantity} / 3" : $"보유 수량<pos=240>{item.quantity}\n\n"+(selected==equipped?"현재 장착 중":"숲에서의 여정을 위한 소지품");
            actionLabel.text=crafting?"제작하기":item.category=="장비"?(selected==equipped?"장착 중":"장착하기"):item.category=="소모품"?"사용하기":"제작 재료";
            action.interactable=crafting ? selected==3?items[6].quantity>=3&&items[2].quantity>=1:items[5].quantity>=3 : item.quantity>0 && item.category!="재료" && selected!=equipped;
            if(crafting && !action.interactable) actionLabel.text="재료 부족";
            capacity.text=crafting?"손으로 만들기  /  제작법 2종":$"소지품  {items.Count(x=>x.quantity>0):00} / 24";
            Component<TMP_Text>("Inventory/Header/Title").text=crafting?"제작":"배낭";
            Component<TMP_Text>("Inventory/InventoryHeading").text=crafting?"제작법":"소지품";
            Component<TMP_Text>("Inventory/InventorySubheading").text=crafting?"숲에서 모은 재료로 여정을 준비하세요":"여정에서 모은 도구와 작은 발견들";
            for(int i=0;i<4;i++) {
                bool active=!crafting&&filter==i;
                Component<UnityEngine.UI.Image>("Inventory/Filters/Filter"+i).color=active?new Color(.29f,.32f,.39f):Color.clear;
                transform.Find("Inventory/Filters/Filter"+i+"/SelectionLine").gameObject.SetActive(active);
                Component<TMP_Text>("Inventory/Filters/Filter"+i+"/Label").color=active?new Color(.97f,.96f,.93f):new Color(.40f,.43f,.47f);
                var glyph=transform.Find("Inventory/Filters/Filter"+i+"/Glyph");
                if(glyph && glyph.TryGetComponent<UnityEngine.UI.Image>(out var graphic)) graphic.color=active?new Color(.97f,.96f,.93f):new Color(.40f,.43f,.47f);
            }
            Component<UnityEngine.UI.Image>("Inventory/Filters/Craft").color=crafting?new Color(.29f,.32f,.39f):Color.clear;
            transform.Find("Inventory/Filters/Craft/SelectionLine").gameObject.SetActive(crafting);
            Component<TMP_Text>("Inventory/Filters/Craft/Label").color=crafting?new Color(.97f,.96f,.93f):new Color(.40f,.43f,.47f);
            Component<UnityEngine.UI.Image>("Inventory/Filters/Craft/Glyph").color=crafting?new Color(.97f,.96f,.93f):new Color(.40f,.43f,.47f);
            healthLabel.text=$"{health:0} / 100"; foodLabel.text=$"{food:0} / 100";
            healthMotion.SetValue(health/100); foodMotion.SetValue(food/100);
            Component<TMP_Text>("Inventory/Equipment/Equipped").text="선택 장비\n"+items[equipped].title;
            Component<UnityEngine.UI.Image>("Inventory/Equipment/Icon").sprite=items[equipped].icon;
        }
    }
}




