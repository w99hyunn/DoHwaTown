using TMPro;
using UnityEngine;

namespace DoHwa.UI
{
    public sealed class UiVolumeReadout : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Slider slider;
        [SerializeField] private TMP_Text label;
        public void Configure(UnityEngine.UI.Slider source, TMP_Text text) { slider = source; label = text; }
        private void OnEnable() { slider.onValueChanged.AddListener(Refresh); Refresh(AudioListener.volume); }
        private void OnDisable() => slider.onValueChanged.RemoveListener(Refresh);
        private void Refresh(float value) => label.text = Mathf.RoundToInt(value * 100) + "%";
    }
}
