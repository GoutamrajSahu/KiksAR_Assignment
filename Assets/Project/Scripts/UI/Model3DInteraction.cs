using System.Collections;
using UnityEngine;


// Handles raw touch gestures and editor mouse inputs for 3D model inspection.
// Implements delta rotation, pinch zoom, and double-tap smooth lerp reset.
public class Model3DInteraction : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Sensitivity of 1-finger horizontal/vertical drag rotation.")]
    public float rotationSpeed = 0.4f;

    [Header("Scale / Pinch Settings")]
    [Tooltip("Minimum allowable model scale factor.")]
    public float minScale = 0.5f;

    [Tooltip("Maximum allowable model scale factor.")]
    public float maxScale = 2.5f;

    [Tooltip("Sensitivity of 2-finger pinch zoom.")]
    public float pinchSensitivity = 0.005f;

    [Header("Reset Animation Settings")]
    [Tooltip("Duration of the smooth lerp reset animation in seconds.")]
    public float resetDuration = 0.35f;

    [Tooltip("Maximum time between two taps to register as a double tap.")]
    public float doubleTapThreshold = 0.3f;

    private Vector3 defaultScale;
    private Quaternion defaultRotation;
    private float lastTapTime = -1f;
    private bool isResetting = false;

    private void Awake()
    {
        defaultScale = transform.localScale;
        defaultRotation = transform.localRotation;
    }

    private void Update()
    {
        if (isResetting) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInputs();
#else
        HandleTouchInputs();
#endif
    }

    private void HandleTouchInputs()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            // Double-tap detection
            if (touch.phase == TouchPhase.Began)
            {
                if (Time.time - lastTapTime < doubleTapThreshold)
                {
                    ResetModel(true);
                    return;
                }
                lastTapTime = Time.time;
            }

            // Single finger drag -> Delta-based proportional rotation
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                RotateModel(delta.x, delta.y);
            }
        }
        else if (Input.touchCount == 2)
        {
            // Two-finger pinch -> Uniform clamped scale
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 prevPos0 = touch0.position - touch0.deltaPosition;
            Vector2 prevPos1 = touch1.position - touch1.deltaPosition;

            float prevDistance = (prevPos0 - prevPos1).magnitude;
            float currentDistance = (touch0.position - touch1.position).magnitude;

            float deltaDistance = currentDistance - prevDistance;
            ScaleModel(deltaDistance * pinchSensitivity);
        }
    }

    private void HandleMouseInputs()
    {
        // Mouse left drag -> Rotation
        if (Input.GetMouseButton(0))
        {
            float deltaX = Input.GetAxis("Mouse X") * 10f;
            float deltaY = Input.GetAxis("Mouse Y") * 10f;
            RotateModel(deltaX, deltaY);
        }

        // Mouse scroll wheel -> Zoom/Scale
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            ScaleModel(scroll * 2f);
        }

        // Double click -> Reset
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastTapTime < doubleTapThreshold)
            {
                ResetModel(true);
            }
            lastTapTime = Time.time;
        }
    }

    private void RotateModel(float deltaX, float deltaY)
    {
        // Rotate around Y axis (proportional to horizontal drag speed)
        transform.Rotate(Vector3.up, -deltaX * rotationSpeed, Space.World);
        // Optional subtle X axis tilt
        transform.Rotate(Vector3.right, deltaY * rotationSpeed, Space.World);
    }

    private void ScaleModel(float deltaScale)
    {
        float newScaleValue = Mathf.Clamp(transform.localScale.x + deltaScale, minScale, maxScale);
        transform.localScale = Vector3.one * newScaleValue;
    }

    private Coroutine resetCoroutine;
    // Public function to reset the 3D model orientation and scale.
    // Can be invoked by UI Buttons (e.g. Reset Button) or external controllers.
    public void ResetModel(bool smooth = true)
    {
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }

        if (smooth && gameObject.activeInHierarchy)
        {
            resetCoroutine = StartCoroutine(SmoothResetRoutine());
        }
        else
        {
            transform.localScale = defaultScale;
            transform.localRotation = defaultRotation;
            isResetting = false;
        }
    }

    // Updates the default baseline scale and rotation.
    // Call this whenever a new 3D model is loaded/spawned into the viewer.
    public void CaptureCurrentTransformAsDefault()
    {
        defaultScale = transform.localScale;
        defaultRotation = transform.localRotation;
    }

    private IEnumerator SmoothResetRoutine()
    {
        isResetting = true;
        float elapsed = 0f;

        Vector3 startScale = transform.localScale;
        Quaternion startRotation = transform.localRotation;

        while (elapsed < resetDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / resetDuration);

            transform.localScale = Vector3.Lerp(startScale, defaultScale, t);
            transform.localRotation = Quaternion.Slerp(startRotation, defaultRotation, t);
            yield return null;
        }

        transform.localScale = defaultScale;
        transform.localRotation = defaultRotation;
        isResetting = false;
        resetCoroutine = null;
    }
}