using System;
using UnityEngine;

namespace DoHwa.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class UiPanelReveal : MonoBehaviour
    {
        private CanvasGroup group;
        private int revision;
        [SerializeField] private RectTransform[] sections = Array.Empty<RectTransform>();
        private Vector2[] rest;

        public void Configure(RectTransform[] targets) => sections = targets;
        private void Awake()
        {
            TryGetComponent<CanvasGroup>(out group);
            rest = new Vector2[sections.Length];
            for (int i = 0; i < sections.Length; i++) rest[i] = sections[i].anchoredPosition;
        }

        private void OnEnable() => Replay();
        public async void Replay()
        {
            int current = ++revision;
            float elapsed = 0;
            double began = Time.realtimeSinceStartupAsDouble;
            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = true;
            for (int i = 0; i < sections.Length; i++) sections[i].anchoredPosition = rest[i] + Vector2.down * 10;
            try
            {
                while (elapsed < .22f)
                {
                    await Awaitable.NextFrameAsync(destroyCancellationToken);
                    if (current != revision) return;
                    elapsed = (float)(Time.realtimeSinceStartupAsDouble - began);
                    float progress = Mathf.Clamp01(elapsed / .14f);
                    group.alpha = 1 - Mathf.Pow(1 - progress, 3);
                    for (int i = 0; i < sections.Length; i++)
                    {
                        float p = Mathf.Clamp01((elapsed - Mathf.Min(i, 4) * .01f) / .18f);
                        sections[i].anchoredPosition = rest[i] + Vector2.down * (10 * Mathf.Pow(1 - p, 3));
                    }
                }
                group.alpha = 1;
                group.interactable = true;
            }
            catch (OperationCanceledException) { }
        }

        public async Awaitable HideAsync()
        {
            int current = ++revision;
            group.interactable = false;
            float initial = group.alpha;
            float elapsed = 0;
            double began = Time.realtimeSinceStartupAsDouble;
            try
            {
                while (elapsed < .10f)
                {
                    await Awaitable.NextFrameAsync(destroyCancellationToken);
                    if (current != revision) return;
                    elapsed = (float)(Time.realtimeSinceStartupAsDouble - began);
                    group.alpha = initial * (1 - Mathf.Clamp01(elapsed / .10f));
                }
            }
            catch (OperationCanceledException) { }
        }

        private void OnDisable()
        {
            revision++;
            for (int i = 0; i < sections.Length; i++) sections[i].anchoredPosition = rest[i];
        }
    }
}

