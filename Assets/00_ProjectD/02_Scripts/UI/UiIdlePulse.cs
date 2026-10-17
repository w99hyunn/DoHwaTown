using UnityEngine;

namespace DoHwa.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class UiIdlePulse : MonoBehaviour
    {
        private CanvasGroup group;
        private float began;
        private void Awake() => TryGetComponent<CanvasGroup>(out group);
        private void OnEnable() => began = Time.unscaledTime;
        private void Update() => group.alpha = .84f + .16f * Mathf.Cos((Time.unscaledTime - began) * 2.2f);
        private void OnDisable() => group.alpha = 1;
    }
}
