using UnityEngine;
using UnityEngine.UI;

namespace DoHwa.UI
{
    public sealed class LoginUtilityPanel : MonoBehaviour
    {
        [SerializeField] private GameObject settings;
        [SerializeField] private GameObject notices;
        [SerializeField] private GameObject language;
        [SerializeField] private GameObject exit;
        [SerializeField] private Slider volume;
        private int transition;

        public void OpenSettings()
        {
            volume.SetValueWithoutNotify(AudioListener.volume);
            OpenPanel(settings);
        }

        public void OpenNotices() => OpenPanel(notices);
        public void OpenLanguage() => OpenPanel(language);
        public void OpenExit() => OpenPanel(exit);
        private void OpenPanel(GameObject panel)
        {
            transition++;
            bool alreadyOpen = panel.activeSelf;
            settings.SetActive(panel == settings);
            notices.SetActive(panel == notices);
            language.SetActive(panel == language);
            exit.SetActive(panel == exit);
            if(alreadyOpen && panel.TryGetComponent<UiPanelReveal>(out var reveal)) reveal.Replay();
        }
        public void CloseSettings() => ClosePanel(settings);
        public void CloseNotices() => ClosePanel(notices);
        public void CloseLanguage() => ClosePanel(language);
        public void CloseExit() => ClosePanel(exit);
        private async void ClosePanel(GameObject panel)
        {
            int current = ++transition;
            if(panel.TryGetComponent<UiPanelReveal>(out var reveal)) await reveal.HideAsync();
            if(panel && current == transition) panel.SetActive(false);
        }
        public void SetVolume(float value) => AudioListener.volume = value;

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
