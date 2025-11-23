using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float dragSpeed = 1f;
    public float zoomSpeed = 0.5f;
    public float minZoom = 1f;
    public float maxZoom = 3f;

    [Header("Camera Bounds")]
    public float clampPadding = 1f; // how far beyond the farm the camera can move (in tiles)

    [Header("References")]
    public TileManager tileManager; // assign in Inspector

    private Camera _cam;
    private Vector3 _dragOrigin;

    void Start()
    {
        _cam = Camera.main;
        CenterAndFitFarm();
    }

    void LateUpdate()
    {
        HandleMouseDrag();
        HandleMouseZoom();
        ClampCamera();
    }

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

    private void ClampCamera()
    {
        int width = tileManager.width;
        int height = tileManager.height;

        float camHeight = _cam.orthographicSize;
        float camWidth = camHeight * _cam.aspect;

        float minX = camWidth - 0.5f - clampPadding;
        float maxX = width - 1 + 0.5f + clampPadding - camWidth;
        float minY = camHeight - 0.5f - clampPadding;
        float maxY = height - 1 + 0.5f + clampPadding - camHeight;

        // ✅ Add slight tolerance so you can still pan at max zoom
        float minPanAllowance = 0.2f; // small buffer (in world units)
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


    void HandleMouseDrag()
    {
        float camWidth = _cam.orthographicSize * _cam.aspect;
        float camHeight = _cam.orthographicSize;

        if ((tileManager.width <= camWidth * 2) && (tileManager.height <= camHeight * 2))
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            _dragOrigin = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 diff = _dragOrigin - _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            _cam.transform.position += new Vector3(diff.x, diff.y, 0) * dragSpeed;
            _dragOrigin = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }
    }

    void HandleMouseZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0)
        {
            _cam.orthographicSize -= scroll * zoomSpeed;
            _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, minZoom, maxZoom);
        }
    }
}
