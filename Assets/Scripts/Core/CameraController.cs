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

    private Camera cam;
    private Vector3 dragOrigin;

    void Start()
    {
        cam = Camera.main;
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

        cam.transform.position = new Vector3(centerX, centerY, cam.transform.position.z);

        float screenAspect = (float)Screen.width / Screen.height;
        float sizeX = width / (2f * screenAspect);
        float sizeY = height / 2f;

        cam.orthographicSize = Mathf.Clamp(Mathf.Max(sizeX, sizeY), minZoom, maxZoom);
    }

    private void ClampCamera()
    {
        int width = tileManager.width;
        int height = tileManager.height;

        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

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

        float clampedX = Mathf.Clamp(cam.transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(cam.transform.position.y, minY, maxY);

        cam.transform.position = new Vector3(clampedX, clampedY, cam.transform.position.z);
    }


    void HandleMouseDrag()
    {
        float camWidth = cam.orthographicSize * cam.aspect;
        float camHeight = cam.orthographicSize;

        if ((tileManager.width <= camWidth * 2) && (tileManager.height <= camHeight * 2))
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            dragOrigin = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 diff = dragOrigin - cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            cam.transform.position += new Vector3(diff.x, diff.y, 0) * dragSpeed;
            dragOrigin = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }
    }

    void HandleMouseZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }
}
