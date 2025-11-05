using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public float tapThreshold = 0.2f;
    public float dragThreshold = 20f;

    private bool isDragging = false;
    private Vector2 startPos;
    private float startTime;

    void Update()
    {
        HandleMouse();

    }

    void HandleMouse()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            startPos = Mouse.current.position.ReadValue();
            startTime = Time.time;
            isDragging = false;
        }

        if (Mouse.current.leftButton.isPressed)
        {
            float distance = Vector2.Distance(startPos, Mouse.current.position.ReadValue());
            if (distance > dragThreshold) isDragging = true;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Debug.Log("Mouse button released");
            float duration = Time.time - startTime;
            if (!isDragging && duration <= tapThreshold)
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                worldPos.z = 0;
                TryInteract(worldPos);
            }
        }
    }

    void TryInteract(Vector3 worldPos)
    {
        Debug.Log("Trying to interact at: " + worldPos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        if (hit == null)
        {
            Debug.Log("No collider detected at " + worldPos);
        }
        else
        {
            Debug.Log("Collider detected: " + hit.name);
        }
        
        if (hit != null && hit.TryGetComponent(out FarmTile tile))
        {
            Debug.Log("Interacted with FarmTile");
            tile.PlantCrop();
            tile.HarvestCrop();
            tile.waterCrop();
        }
    }
}
