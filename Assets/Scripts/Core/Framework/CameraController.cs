using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float dragSpeed = 1f;
    public float zoomSpeed = 0.5f;
    public float minZoom = 1f;
    public float maxZoom = 3f;

    [Header("Camera Bounds")]
    public float clampPadding = 1f;

    [Header("References")]
    public TileManager tileManager;

    private Camera _cam;
    private Vector3 _dragOrigin;

    // Touch zoom helpers
    private float _previousPinchDistance;

    void Start()
    {
        _cam = Camera.main;
        CenterAndFitFarm();
    }

    public void UpdateCamera()
    {
        HandleDrag();
        HandleZoom();
        ClampCamera();
    }

    // =========================
    // INITIAL SETUP
    // =========================

    private void CenterAndFitFarm()
    {
        int width = tileManager.width;
        int height = tileManager.height;

        float centerX = (width - 1) / 2f;
        float centerY = (height - 1) / 2f;

        _cam.transform.position = new Vector3(centerX, centerY, _cam.transform.position.z);

        float screenAspect = (float)Screen.width / Screen.height;
        float sizeX = width / (2f * screenAspect);
        float sizeY = height / 2f;

        _cam.orthographicSize = Mathf.Clamp(Mathf.Max(sizeX, sizeY), minZoom, maxZoom);
    }

    // =========================
    // DRAG (Mouse + Touch)
    // =========================

    void HandleDrag()
    {
        float camWidth = _cam.orthographicSize * _cam.aspect;
        float camHeight = _cam.orthographicSize;

        if (tileManager.width <= camWidth * 2 &&
            tileManager.height <= camHeight * 2)
            return;

        // 🖱 Mouse drag
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
                _dragOrigin = ScreenToWorld(Mouse.current.position.ReadValue());

            if (Mouse.current.leftButton.isPressed)
                DragCamera(Mouse.current.position.ReadValue());
        }

        // 📱 One-finger drag
        if (Touchscreen.current != null &&
            Touchscreen.current.touches.Count == 1)
        {
            TouchControl touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
                _dragOrigin = ScreenToWorld(touch.position.ReadValue());

            if (touch.press.isPressed)
                DragCamera(touch.position.ReadValue());
        }
    }

    void DragCamera(Vector2 screenPos)
    {
        Vector3 currentWorldPos = ScreenToWorld(screenPos);
        Vector3 diff = _dragOrigin - currentWorldPos;

        _cam.transform.position += new Vector3(diff.x, diff.y, 0f) * dragSpeed;
        _dragOrigin = currentWorldPos;
    }

    // =========================
    // ZOOM (Mouse + Pinch)
    // =========================

    void HandleZoom()
    {
        // 🖱 Mouse wheel zoom
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (scroll != 0)
            {
                ZoomCamera(scroll * zoomSpeed);
            }
        }

        // 📱 Pinch zoom
        if (Touchscreen.current != null &&
            Touchscreen.current.touches.Count == 2)
        {
            var touch0 = Touchscreen.current.touches[0];
            var touch1 = Touchscreen.current.touches[1];

            Vector2 p0 = touch0.position.ReadValue();
            Vector2 p1 = touch1.position.ReadValue();

            float currentDistance = Vector2.Distance(p0, p1);

            if (touch0.press.wasPressedThisFrame || touch1.press.wasPressedThisFrame)
            {
                _previousPinchDistance = currentDistance;
                return;
            }

            float delta = currentDistance - _previousPinchDistance;
            _previousPinchDistance = currentDistance;

            ZoomCamera(delta * zoomSpeed * 0.01f);
        }
    }

    void ZoomCamera(float delta)
    {
        _cam.orthographicSize -= delta;
        _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, minZoom, maxZoom);
    }

    // =========================
    // CLAMPING
    // =========================

    private void ClampCamera()
    {
        int width = tileManager.width;
        int height = tileManager.height;

        float camHeight = _cam.orthographicSize;
        float camWidth = camHeight * _cam.aspect;

        float minX = camWidth - 0.5f - clampPadding;
        float maxX = width - 0.5f + clampPadding - camWidth;
        float minY = camHeight - 0.5f - clampPadding;
        float maxY = height - 0.5f + clampPadding - camHeight;

        float minPanAllowance = 0.2f;

        if (maxX - minX < minPanAllowance)
        {
            float centerX = (width - 1) / 2f;
            minX = centerX - minPanAllowance / 2f;
            maxX = centerX + minPanAllowance / 2f;
        }

        if (maxY - minY < minPanAllowance)
        {
            float centerY = (height - 1) / 2f;
            minY = centerY - minPanAllowance / 2f;
            maxY = centerY + minPanAllowance / 2f;
        }

        float clampedX = Mathf.Clamp(_cam.transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(_cam.transform.position.y, minY, maxY);

        _cam.transform.position = new Vector3(clampedX, clampedY, _cam.transform.position.z);
    }

    // =========================
    // HELPERS
    // =========================

    Vector3 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 world = _cam.ScreenToWorldPoint(screenPos);
        world.z = 0f;
        return world;
    }
}
