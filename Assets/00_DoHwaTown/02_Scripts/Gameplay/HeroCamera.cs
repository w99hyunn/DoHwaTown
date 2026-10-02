using UnityEngine;

namespace DoHwa
{
    [DefaultExecutionOrder(100)]
    public sealed class HeroCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Transform spriteVisual;
        [SerializeField] private float distance = 28f;
        [SerializeField] private float pitch = 48f;
        [SerializeField] private float followSharpness = 12f;
        private Vector3 focus;

        public void Configure(Transform hero, Transform visual)
        {
            target = hero;
            spriteVisual = visual;
            Snap();
        }

        private void Start() => Snap();

        private void Snap()
        {
            focus = target.position + Vector3.up * 0.65f;
            ApplyPose();
        }

        private void LateUpdate()
        {
            focus = Vector3.Lerp(focus, target.position + Vector3.up * 0.65f, 1f - Mathf.Exp(-followSharpness * Time.deltaTime));
            ApplyPose();
        }

        private void ApplyPose()
        {
            transform.rotation = Quaternion.Euler(pitch, 0f, 0f);
            transform.position = focus - transform.forward * distance;
            // Camera-aligned sprite plane keeps pixel proportions intact in the perspective view.
            spriteVisual.rotation = transform.rotation;
        }
    }
}
