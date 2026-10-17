using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DoHwa.UI
{
    public enum EquipmentPart { None=-1, Weapon, OffHand, Head, Body, Legs, Feet, Necklace, RingLeft, RingRight, Bag }

    public sealed class EquipmentPanel : MonoBehaviour
    {
        public static readonly string[] PartNames={"무기","보조손","머리","상의","하의","신발","목걸이","반지 (왼쪽)","반지 (오른쪽)","가방"};
        [SerializeField] private RpgUiController owner;
        [SerializeField] private Image[] faces, icons;
        [SerializeField] private TMP_Text[] names;
        [SerializeField] private TMP_Text summary;
        public void Configure(RpgUiController controller,Image[] backgrounds,Image[] art,TMP_Text[] titles,TMP_Text count)
        { owner=controller;faces=backgrounds;icons=art;names=titles;summary=count; }
        public void SelectPart(int part) => owner.SelectEquipmentPart(part);
        public void RefreshView(int selected)
        {
            int count=0;
            for(int i=0;i<faces.Length;i++) {
                int id=owner.EquippedAt(i);bool filled=id>=0;
                icons[i].enabled=filled;icons[i].sprite=filled?owner.Catalog[id].icon:null;
                names[i].text=filled?owner.Catalog[id].title:"미착용";
                faces[i].color=i==selected?new Color(.68f,.82f,.94f):Color.white;
                if(filled)count++;
            }
            summary.text=$"착용 중  {count} / {faces.Length}";
            var candidates=owner.Catalog.Select((item,id)=>(item,id)).Where(x=>(int)x.item.equipmentPart==selected && x.item.quantity>0).ToArray();
            transform.Find("CandidateHeading").TryGetComponent<TMP_Text>(out var heading);
            heading.text=candidates.Length==0?PartNames[selected]+" / 소지 장비 없음":PartNames[selected]+" / 교체할 장비";
            foreach(var button in transform.GetComponentsInChildren<Button>(true).Where(b=>b.name.StartsWith("Candidate"))) {
                int id=int.Parse(button.name.Substring(9));
                int index=System.Array.FindIndex(candidates,x=>x.id==id);
                button.gameObject.SetActive(index>=0);
                if(index>=0) {
                    var rect=(RectTransform)button.transform;
                    rect.anchoredPosition=new Vector2(index*246,-688);
                }
            }
        }
    }
}


