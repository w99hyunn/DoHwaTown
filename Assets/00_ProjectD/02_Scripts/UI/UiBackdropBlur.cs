using UnityEngine;
using UnityEngine.UI;

namespace DoHwa.UI
{
    public sealed class UiBackdropBlur : MonoBehaviour
    {
        [SerializeField] private Shader blurShader;
        [SerializeField] private RawImage backdrop;
        private Material blur;
        private RenderTexture result;

        public void Configure(Shader shader, RawImage image) { blurShader=shader; backdrop=image; }
        public async Awaitable CaptureAsync()
        {
            backdrop.enabled=false;
            await Awaitable.EndOfFrameAsync();
            if(!this || !isActiveAndEnabled) return;
            if(!blur) blur=new Material(blurShader);
            if(result) { result.Release(); Destroy(result); }
            int width=Mathf.Max(1,Screen.width/4), height=Mathf.Max(1,Screen.height/4);
            var screen=RenderTexture.GetTemporary(Screen.width,Screen.height,0,RenderTextureFormat.ARGB32);
            var work=RenderTexture.GetTemporary(width,height,0,RenderTextureFormat.ARGB32);
            result=new RenderTexture(width,height,0,RenderTextureFormat.ARGB32){filterMode=FilterMode.Bilinear};
            result.Create();
            try {
                ScreenCapture.CaptureScreenshotIntoRenderTexture(screen);
                Graphics.Blit(screen,result);
                for(int i=0;i<3;i++) {
                    blur.SetVector("_Direction",new Vector4(1f/width,0,0,0)); Graphics.Blit(result,work,blur);
                    blur.SetVector("_Direction",new Vector4(0,1f/height,0,0)); Graphics.Blit(work,result,blur);
                }
                backdrop.texture=result;backdrop.enabled=true;
            }
            finally { RenderTexture.ReleaseTemporary(screen);RenderTexture.ReleaseTemporary(work); }
        }
        public void Hide() => backdrop.enabled=false;
        private void OnDestroy() { if(result){result.Release();Destroy(result);} if(blur)Destroy(blur); }
    }
}
