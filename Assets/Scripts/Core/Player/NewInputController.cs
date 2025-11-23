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
    private GameObject hoveredCharacter;
    private bool isPlayer;
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    void Update()
    {
        HandleMouse();

    }

    void HandleMouse()
    {
        
        
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            if (hoveredTile != null)
            {
                hoveredTile.isHovered = false;
                hoveredTile = null;
            }
            if (hoveredCharacter != null)
            {
                if (isPlayer)
                {
                    hoveredCharacter.GetComponent<PlayerData>().hovered = false;
                }
                else
                {
                    hoveredCharacter.GetComponent<WorkerData>().hovered = false; 
                }
                isPlayer = false;
                hoveredCharacter = null;
            }
            return;
        }


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

        if (hoveredCharacter != null)
        {
            if (isPlayer)
            {
                hoveredCharacter.GetComponent<PlayerData>().hovered = false;
            }
            else
            {
                hoveredCharacter.GetComponent<WorkerData>().hovered = false; 
            }
            isPlayer = false;
            hoveredCharacter = null;
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
        if (hit != null && hit.TryGetComponent(out PlayerData player))
        {
            hoveredCharacter = player.gameObject;
            isPlayer = true;
            player.hovered = true;
        }
        else if (hit != null&&hit.TryGetComponent(out WorkerData worker))
        {
            hoveredCharacter = worker.gameObject;
            worker.hovered = true;
        }
        else if (hit != null && hit.TryGetComponent(out FarmTile tile))
        {
            tile.isHovered = true;
            hoveredTile = tile;
        }
    }
    void TryInteract(Vector3 worldPos)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        if (hit != null)
        {
            if (hit.TryGetComponent(out PlayerData player))
            {
                gameManager.SelectCharacter(player.gameObject);
            }
            else if (hit.TryGetComponent(out WorkerData worker))
            {
                gameManager.SelectCharacter(worker.gameObject);
            }
        }
        
        if (gameManager.selectedCharacter.TryGetComponent(out PlayerData data) 
            && hit != null && hit.TryGetComponent(out FarmTile tile))
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<PlayerInteraction>().addTile(tile.gameObject);
            }
        }
    }