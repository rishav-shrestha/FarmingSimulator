using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float minZoom = 1f;
    public float maxZoom = 6f;

    [Header("PC Speed Settings")]
    public float pcDragSpeed = 1.2f;
    public float pcZoomSpeed = 1.0f;

    [Header("Mobile Speed Settings")]
    public float mobileDragSpeed = 1.0f;
    public float mobileZoomSpeed = 1.0f;

    [Header("Inertia Settings")]
    public bool enableInertia = true;
    public float dragInertia = 0.88f;
    public float zoomInertia = 0.85f;

    [Header("Camera Bounds")]
    public float clampPadding = 1f;

    [Header("References")]
    public TileManager tileManager;

    private Camera _cam;
    private Vector3 _dragVelocity;
    private float _zoomVelocity;
    private float _lastPinchDistance;

    private void OnEnable() => EnhancedTouchSupport.Enable();
    private void OnDisable() => EnhancedTouchSupport.Disable();

    void Start()
    {
        _cam = Camera.main;
        CenterCamera();
    }

    // Called externally
    public void UpdateCamera()
    {
        HandleDrag();
        HandleZoom();
        ApplyInertia();
        ClampCamera();
    }

    // =========================
    // Drag (PC + Mobile)
    // =========================
    void HandleDrag()
    {
        // -------- MOBILE --------
        if (Touch.activeTouches.Count == 1)
        {
            var touch = Touch.activeTouches[0];
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                Vector2 delta = touch.delta;
                float zoomFactor = _cam.orthographicSize;

                Vector3 move = new Vector3(
                    -delta.x * zoomFactor * mobileDragSpeed * 0.001f,
                    -delta.y * zoomFactor * mobileDragSpeed * 0.001f,
                    0f
                );

                _cam.transform.position += move;
                _dragVelocity = move;
            }
        }
        // -------- PC --------
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            float zoomFactor = _cam.orthographicSize;

            Vector3 move = new Vector3(
                -delta.x * zoomFactor * pcDragSpeed * 0.001f,
                -delta.y * zoomFactor * pcDragSpeed * 0.001f,
                0f
            );

            _cam.transform.position += move;
            _dragVelocity = move;
        }
    }

    // =========================
    // Zoom (PC + Mobile)
    // =========================
    void HandleZoom()
    {
        // -------- MOBILE PINCH --------
        if (Touch.activeTouches.Count == 2)
        {
            var t0 = Touch.activeTouches[0];
            var t1 = Touch.activeTouches[1];

            float distance = Vector2.Distance(t0.screenPosition, t1.screenPosition);

            if (t0.phase == UnityEngine.InputSystem.TouchPhase.Began ||
                t1.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                _lastPinchDistance = distance;
                _zoomVelocity = 0f;
                return;
            }

            float delta = distance - _lastPinchDistance;
            _lastPinchDistance = distance;

            _zoomVelocity = delta * mobileZoomSpeed * 0.001f;
            _cam.orthographicSize = Mathf.Clamp(
                _cam.orthographicSize - _zoomVelocity,
                minZoom,
                maxZoom
            );
        }

        // -------- PC SCROLL --------
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (!Mathf.Approximately(scroll, 0f))
            {
                _zoomVelocity = scroll * pcZoomSpeed * 0.01f;
                _cam.orthographicSize = Mathf.Clamp(
                    _cam.orthographicSize - _zoomVelocity,
                    minZoom,
                    maxZoom
                );
            }
        }
    }

    // =========================
    // Inertia
    // =========================
    void ApplyInertia()
    {
        if (!enableInertia) return;

        bool dragging =
            Touch.activeTouches.Count == 1 ||
            (Mouse.current != null && Mouse.current.leftButton.isPressed);

        if (!dragging && _dragVelocity.magnitude > 0.0001f)
        {
            _cam.transform.position += _dragVelocity;
            _dragVelocity *= dragInertia;
        }

        bool zooming =
            Touch.activeTouches.Count == 2 ||
            (Mouse.current != null &&
             !Mathf.Approximately(Mouse.current.scroll.ReadValue().y, 0f));

        if (!zooming && Mathf.Abs(_zoomVelocity) > 0.0001f)
        {
            _cam.orthographicSize = Mathf.Clamp(
                _cam.orthographicSize - _zoomVelocity,
                minZoom,
                maxZoom
            );
            _zoomVelocity *= zoomInertia;
        }
    }

    // =========================
    // Clamp Camera
    // =========================
    void ClampCamera()
    {
        if (tileManager == null) return;

        float camHeight = _cam.orthographicSize;
        float camWidth = camHeight * _cam.aspect;

        float minX = camWidth - 0.5f - clampPadding;
        float maxX = tileManager.width - 0.5f + clampPadding - camWidth;
        float minY = camHeight - 0.5f - clampPadding;
        float maxY = tileManager.height - 0.5f + clampPadding - camHeight;

        float minPanAllowance = 0.2f;

        if (maxX - minX < minPanAllowance)
        {
            float centerX = (tileManager.width - 1) / 2f;
            minX = centerX - minPanAllowance / 2f;
            maxX = centerX + minPanAllowance / 2f;
        }

        if (maxY - minY < minPanAllowance)
        {
            float centerY = (tileManager.height - 1) / 2f;
            minY = centerY - minPanAllowance / 2f;
            maxY = centerY + minPanAllowance / 2f;
        }

        _cam.transform.position = new Vector3(
            Mathf.Clamp(_cam.transform.position.x, minX, maxX),
            Mathf.Clamp(_cam.transform.position.y, minY, maxY),
            _cam.transform.position.z
        );
    }

    // =========================
    // Helpers
    // =========================
    void CenterCamera()
    {
        if (tileManager == null) return;

        _cam.transform.position = new Vector3(
            tileManager.width / 2f,
            tileManager.height / 2f,
            _cam.transform.position.z
        );
    }
}
