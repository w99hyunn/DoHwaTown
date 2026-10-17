using UnityEngine;
using UnityEngine.EventSystems;

namespace DoHwa.UI
{
    [RequireComponent(typeof(UnityEngine.UI.Selectable))]
    public sealed class UiInteractionMotion : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private UnityEngine.UI.Selectable control;
        private bool hover, pressed;
        [SerializeField] private UnityEngine.UI.Image feedback;
        [SerializeField] private CanvasGroup visualGroup;
        public void Configure(UnityEngine.UI.Image image, CanvasGroup group) { feedback = image; visualGroup = group; }
        private void Awake() { TryGetComponent<UnityEngine.UI.Selectable>(out control); }
        private void Update()
        {
            bool interactive = control.IsInteractable();
            bool focused = EventSystem.current && EventSystem.current.currentSelectedGameObject == gameObject;
            float blend = 1 - Mathf.Exp(-22 * Time.unscaledDeltaTime);
            if (feedback) {
                float alpha = !interactive ? 0 : pressed ? .28f : hover ? .16f : focused ? .10f : 0;
                feedback.color = Color.Lerp(feedback.color, new Color(.36f, .40f, .44f, alpha), blend);
            }
            if (visualGroup) visualGroup.alpha = interactive ? 1 : .42f;
        }
        public void OnPointerEnter(PointerEventData data) => hover = true;
        public void OnPointerExit(PointerEventData data) { hover = false; pressed = false; }
        public void OnPointerDown(PointerEventData data) => pressed = true;
        public void OnPointerUp(PointerEventData data) => pressed = false;
        private void OnDisable() { hover = pressed = false; }
    }
}




