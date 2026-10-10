using UnityEngine;

namespace DoHwa
{
    public sealed class BlossomDrift : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float height = 6f;

        public void Configure(Transform followTarget) => target = followTarget;

        private void LateUpdate()
        {
            // World-space particles keep their flight path when the emitter follows the hero.
            transform.position = target.position + Vector3.up * height;
        }
    }
}
