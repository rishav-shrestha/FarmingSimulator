using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class NewInputController : MonoBehaviour
{
    public float tapThreshold = 0.2f;
    public float dragThreshold = 20f;

    private bool isDragging = false;
    private Vector2 startPos;
    private float startTime;
    private FarmTile hoveredTile;

    void Update()
    {
        HandleMouse();

    }

    void HandleMouse()
    {
    
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;


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

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        worldPos.z = 0;
        if(hoveredTile != null)
        {
            hoveredTile.isHovered = false;
            hoveredTile = null;
        }
        Find(worldPos); 
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            float duration = Time.time - startTime;
            if (!isDragging && duration <= tapThreshold)
            {
                worldPos.z = 0;
                TryInteract(worldPos);
            }
        }
    }

    void Find(Vector3 worldpos)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldpos);
        if (hit != null && hit.TryGetComponent(out FarmTile tile))
        {
            hoveredTile = tile;
            tile.isHovered = true;    
        }
    }
    void TryInteract(Vector3 worldPos)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        if (hit != null && hit.TryGetComponent(out FarmTile tile))
        {
           GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<PlayerInteraction>().addTile(tile.gameObject);
        }
    }
}
