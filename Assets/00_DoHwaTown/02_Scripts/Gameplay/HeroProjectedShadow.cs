using UnityEngine;

namespace DoHwa
{
    [DefaultExecutionOrder(150)]
    public sealed class HeroProjectedShadow : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] sources;
        [SerializeField] private SpriteRenderer[] shadows;
        private MaterialPropertyBlock properties;
        private static readonly int GroundHeight = Shader.PropertyToID("_GroundHeight");

        public void Configure(SpriteRenderer[] sourceLayers, SpriteRenderer[] shadowLayers)
        {
            sources = sourceLayers;
            shadows = shadowLayers;
        }

        private void Awake() => properties = new MaterialPropertyBlock();

        private void LateUpdate()
        {
            properties.SetFloat(GroundHeight, transform.position.y + .025f);
            for (int i = 0; i < sources.Length; i++)
            {
                var source = sources[i];
                var shadow = shadows[i];
                shadow.enabled = source.enabled && source.gameObject.activeInHierarchy;
                shadow.sprite = source.sprite;
                shadow.flipX = source.flipX;
                shadow.flipY = source.flipY;
                shadow.color = new Color(1f, 1f, 1f, source.color.a);
                shadow.transform.SetPositionAndRotation(source.transform.position, source.transform.rotation);
                shadow.transform.localScale = source.transform.lossyScale;
                shadow.SetPropertyBlock(properties);
            }
        }
    }
}
