using UnityEngine;

namespace DoHwa.UI
{
    [CreateAssetMenu(menuName = "DoHwa/UI/Image Theme")]
    public sealed class UiImageTheme : ScriptableObject
    {
        public Sprite surface;
        public Sprite primarySurface, secondarySurface, selectedTab, closeSurface;
        public Color primaryInk = new(.97f, .96f, .93f);
        public Sprite confirmIcon;
        public Sprite cancelIcon;
        public Color ink = new(.20f, .24f, .28f);
        public Color accent = new(.28f, .32f, .37f);
        public Color hover = new(.96f, .97f, .98f);
        public Color pressed = new(.88f, .90f, .93f);
        public float cornerSize = 12;
        public float iconSize = 24;
        public float iconInset = 24;
        public float transitionSpeed = 22;
    }
}
