using UnityEngine;
using UnityEngine.InputSystem;

namespace DoHwa
{
    [DefaultExecutionOrder(100)]
    public sealed class HeroCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Transform spriteVisual;
        [SerializeField] private float distance = 28f;
        [SerializeField] private float pitch = 48f;
        [SerializeField] private float yaw;
        [SerializeField] private float followSharpness = 12f;
        [SerializeField] private float nearDistance = 8f;
        [SerializeField] private float farDistance = 26f;
        [SerializeField] private float nearPitch = 36f;
        [SerializeField] private float farPitch = 52f;
        [SerializeField] private float zoomStep = 0.14f;
        [SerializeField] private float rotationSensitivity = 0.18f;
        [SerializeField] private float zoomSharpness = 10f;
        private float zoom;
        private float desiredZoom;
        private InputAction scroll;
        private InputAction orbit;
        private InputAction pointerDelta;
        private Vector3 focus;

        private void Awake()
        {
            zoom = desiredZoom = Mathf.InverseLerp(nearDistance, farDistance, distance);
            scroll = new InputAction("Camera Zoom", InputActionType.Value, "<Mouse>/scroll/y");
            orbit = new InputAction("Camera Orbit", InputActionType.Button, "<Mouse>/rightButton");
            pointerDelta = new InputAction("Camera Pointer Delta", InputActionType.Value, "<Mouse>/delta");
        }

        private void OnEnable()
        {
            scroll.Enable();
            orbit.Enable();
            pointerDelta.Enable();
        }

        private void OnDisable()
        {
            scroll.Disable();
            orbit.Disable();
            pointerDelta.Disable();
        }

        private void OnDestroy()
        {
            scroll.Dispose();
            orbit.Dispose();
            pointerDelta.Dispose();
        }

        public void SetZoom(float amount) => desiredZoom = Mathf.Clamp01(amount);

        public void Rotate(float horizontalPixels) => yaw += horizontalPixels * rotationSensitivity;

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
            SetZoom(desiredZoom - scroll.ReadValue<float>() * zoomStep);
            if (orbit.IsPressed()) Rotate(pointerDelta.ReadValue<Vector2>().x);
            zoom = Mathf.Lerp(zoom, desiredZoom, 1f - Mathf.Exp(-zoomSharpness * Time.deltaTime));
            distance = Mathf.Lerp(nearDistance, farDistance, zoom);
            pitch = Mathf.Lerp(nearPitch, farPitch, Mathf.SmoothStep(0f, 1f, zoom));
            focus = Vector3.Lerp(focus, target.position + Vector3.up * 0.65f, 1f - Mathf.Exp(-followSharpness * Time.deltaTime));
            ApplyPose();
        }

        private void ApplyPose()
        {
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
            transform.position = focus - transform.forward * distance;
            // Camera-aligned sprite plane keeps pixel proportions intact in the perspective view.
            spriteVisual.rotation = transform.rotation;
        }
    }
}
