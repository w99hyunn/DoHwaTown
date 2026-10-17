using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DoHwa.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class UiIllustratedButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image face;
        [SerializeField] private TMP_Text label;
        private Button button;
        private bool hovered, pressed;

        public void Configure(Image image, TMP_Text text) { face = image; label = text; }
        private void Awake() => TryGetComponent<Button>(out button);
        private void Update()
        {
            bool interactive = button.IsInteractable();
            bool focused = EventSystem.current && EventSystem.current.currentSelectedGameObject == gameObject;
            float shade = !interactive ? .65f : pressed ? .74f : hovered || focused ? 1f : .9f;
            float blend = 1 - Mathf.Exp(-24 * Time.unscaledDeltaTime);
            face.color = Color.Lerp(face.color, new Color(shade, shade, shade, interactive ? 1 : .62f), blend);
            label.color = Color.Lerp(label.color, new Color(.98f, .96f, .91f, interactive ? 1 : .65f), blend);
        }
        public void OnPointerEnter(PointerEventData data) => hovered = true;
        public void OnPointerExit(PointerEventData data) { hovered = false; pressed = false; }
        public void OnPointerDown(PointerEventData data) { if (data.button == PointerEventData.InputButton.Left) pressed = true; }
        public void OnPointerUp(PointerEventData data) => pressed = false;
        private void OnDisable() { hovered = false; pressed = false; }
    }
}
