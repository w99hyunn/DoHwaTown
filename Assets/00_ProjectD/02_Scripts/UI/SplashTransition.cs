using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DoHwa.UI
{
    public sealed class SplashTransition : MonoBehaviour
    {
        [SerializeField] private CanvasGroup content;
        [SerializeField] private float noticeDuration = 3f;
        [SerializeField] private float fadeDuration = 0.45f;

        private async void Start()
        {
            var cancellationToken = destroyCancellationToken;
            try
            {
                var startedAt = Time.unscaledTime;
                while (Time.unscaledTime - startedAt < noticeDuration)
                    await Awaitable.NextFrameAsync(cancellationToken);

                startedAt = Time.unscaledTime;
                while (Time.unscaledTime - startedAt < fadeDuration)
                {
                    content.alpha = 1f - Mathf.Clamp01((Time.unscaledTime - startedAt) / fadeDuration);
                    await Awaitable.NextFrameAsync(cancellationToken);
                }

                content.alpha = 0f;
                await Awaitable.FromAsyncOperation(SceneManager.LoadSceneAsync("Login"), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Leaving the scene cancels the pending transition.
            }
        }
    }
}
