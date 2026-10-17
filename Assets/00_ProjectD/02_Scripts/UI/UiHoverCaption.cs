using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DoHwa.UI
{
    public sealed class UiHoverCaption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text caption;
        private bool hovered;
        public void Configure(TMP_Text text) { caption = text; caption.alpha = 0; }
        private void OnEnable() => hovered = false;
        public void OnPointerEnter(PointerEventData data) => hovered = true;
        public void OnPointerExit(PointerEventData data) => hovered = false;
        private void Update() => caption.alpha = Mathf.MoveTowards(caption.alpha, hovered ? 1 : 0, Time.unscaledDeltaTime * 8);
    }
}
