using System;
using UnityEngine;

namespace DoHwa.UI
{
    public sealed class LoginEntry : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Button startButton;
        [SerializeField] private TMPro.TMP_Text startLabel;
        public void Configure(UnityEngine.UI.Button button,TMPro.TMP_Text label) { startButton=button; startLabel=label; }
        public async void Enter()
        {
            if(!startButton.interactable) return;
            startButton.interactable=false;
            startLabel.text="숲으로 들어가는 중...";
            try { await Awaitable.FromAsyncOperation(UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Ingame"),destroyCancellationToken); }
            catch(OperationCanceledException) { }
            catch(Exception exception) { Debug.LogException(exception); startButton.interactable=true; startLabel.text="다시 시작"; }
        }
    }
}
