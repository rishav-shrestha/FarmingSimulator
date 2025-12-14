using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [Header("Input Settings")]
    public float tapThreshold = 0.2f;
    public float dragThreshold = 20f;

    [Header("State")]
    [SerializeField] private bool isDragging;
    [SerializeField] private Vector2 startPos;
    [SerializeField] private float startTime;

    [Header("Hovered Objects")]
    [SerializeField] private FarmTile hoveredTile;
    [SerializeField] private GameObject hoveredCharacter;
    [SerializeField] private StorageUnit hoveredStorageUnit;
    [SerializeField] private Plot hoveredPlot;
    [SerializeField] private bool isPlayer;

    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private PlotManager plotManager;

    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void Start()
    {
        plotManager = GetComponent<PlotManager>();
        gameManager = GetComponent<GameManager>();
        cameraController = GetComponent<CameraController>();
    }

    void Update()
    {
        if (gameManager.GetGameMode() == GameManager.GameMode.Pause ||
            gameManager.GetGameMode() == GameManager.GameMode.Map)
            return;

        if (IsPointerOverUI())
        {
            ClearHoveredObjects();
            return;
        }

        HandlePointerPress();
        HandlePointerDrag();
        UpdateHoveredObjects();
        HandlePointerRelease();

        cameraController.UpdateCamera();
    }

    // =========================
    // POINTER INPUT (Mouse + Touch)
    // =========================

    bool PointerDownThisFrame()
    {
        return (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
               (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);
    }

    bool PointerHeld()
    {
        return (Mouse.current != null && Mouse.current.leftButton.isPressed) ||
               (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed);
    }

    bool PointerReleasedThisFrame()
    {
        return (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame) ||
               (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame);
    }

    Vector2 GetPointerPosition()
    {
        if (Touchscreen.current != null)
            return Touchscreen.current.primaryTouch.position.ReadValue();

        return Mouse.current.position.ReadValue();
    }

    // =========================
    // UI BLOCKING
    // =========================

    bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        if (Touchscreen.current != null)
            return EventSystem.current.IsPointerOverGameObject(
                Touchscreen.current.primaryTouch.touchId.ReadValue()
            );

        return EventSystem.current.IsPointerOverGameObject();
    }

    // =========================
    // INPUT HANDLING
    // =========================

    void HandlePointerPress()
    {
        if (PointerDownThisFrame())
        {
            startPos = GetPointerPosition();
            startTime = Time.time;
            isDragging = false;
        }
    }

    void HandlePointerDrag()
    {
        if (PointerHeld())
        {
            float distance = Vector2.Distance(startPos, GetPointerPosition());
            if (distance > dragThreshold)
                isDragging = true;
        }
    }

    void HandlePointerRelease()
    {
        if (PointerReleasedThisFrame())
        {
            float duration = Time.time - startTime;
            if (!isDragging && duration <= tapThreshold)
            {
                Vector3 worldPos = mainCam.ScreenToWorldPoint(GetPointerPosition());
                worldPos.z = 0;

                if (gameManager.GetGameMode() == GameManager.GameMode.Play)
                    TryInteractPlayMode(worldPos);
                else if (gameManager.GetGameMode() == GameManager.GameMode.Edit)
                    TryInteractEditMode(worldPos);
            }
        }
    }

    void UpdateHoveredObjects()
    {
        if (Touchscreen.current != null) return; // No hover on mobile

        Vector3 worldPos = mainCam.ScreenToWorldPoint(GetPointerPosition());
        worldPos.z = 0;

        ClearHoveredObjects();
        Find(worldPos);
    }

    // =========================
    // HOVER / FIND
    // =========================

    void Find(Vector3 worldPos)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (!HoverCharacter(hit))
            HoverTile(hit);

        if (!HoverStorageUnit(hit))
            HoverPlot(hit);
    }

    bool HoverCharacter(Collider2D hit)
    {
        if (hit != null && hit.TryGetComponent(out PlayerData player))
        {
            hoveredCharacter = player.gameObject;
            isPlayer = true;
            player.hovered = true;
            return true;
        }

        if (hit != null && hit.TryGetComponent(out WorkerData worker))
        {
            hoveredCharacter = worker.gameObject;
            worker.hovered = true;
            return true;
        }

        return false;
    }

    void HoverTile(Collider2D hit)
    {
        if (hit != null && hit.TryGetComponent(out FarmTile tile))
        {
            hoveredTile = tile;
            tile.isHovered = true;
        }
    }

    bool HoverStorageUnit(Collider2D hit)
    {
        if (hit != null && hit.TryGetComponent(out StorageUnit unit))
        {
            hoveredStorageUnit = unit;
            unit.hovered = true;
            return true;
        }
        return false;
    }

    void HoverPlot(Collider2D hit)
    {
        if (hit != null && hit.TryGetComponent(out Plot plot))
        {
            hoveredPlot = plot;
            plot.hovered = true;
        }
    }

    void ClearHoveredObjects()
    {
        if (hoveredTile != null)
        {
            hoveredTile.isHovered = false;
            hoveredTile = null;
        }

        if (hoveredCharacter != null)
        {
            if (isPlayer)
                hoveredCharacter.GetComponent<PlayerData>().hovered = false;
            else
                hoveredCharacter.GetComponent<WorkerData>().hovered = false;

            hoveredCharacter = null;
            isPlayer = false;
        }

        if (hoveredStorageUnit != null)
        {
            hoveredStorageUnit.hovered = false;
            hoveredStorageUnit = null;
        }

        if (hoveredPlot != null)
        {
            hoveredPlot.hovered = false;
            hoveredPlot = null;
        }
    }

    // =========================
    // INTERACTION LOGIC (UNCHANGED)
    // =========================

    void TryInteractPlayMode(Vector3 worldPos)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        SelectCharacter(hit);
        SelectTile(hit);
        SelectUnit(hit);
    }

    void SelectCharacter(Collider2D hit)
    {
        if (hit == null) return;

        if (hit.TryGetComponent(out PlayerData player))
            gameManager.SelectCharacter(player.gameObject);
        else if (hit.TryGetComponent(out WorkerData worker))
            gameManager.SelectCharacter(worker.gameObject);
    }

    void SelectTile(Collider2D hit)
    {
        if (hit == null) return;

        if (gameManager.selectedCharacter.TryGetComponent(out PlayerData _) &&
            hit.TryGetComponent(out FarmTile tile))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<PlayerInteraction>().AddTile(tile.gameObject);
        }
    }

    void SelectUnit(Collider2D hit)
    {
        if (hit != null && hit.TryGetComponent(out StorageUnit unit))
            plotManager.selectedUnit = unit;
    }

    void TryInteractEditMode(Vector3 worldPos)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        InteractNormalMode(hit);
        InteractAddRemoveMode(hit);
    }

    void InteractAddRemoveMode(Collider2D hit)
    {
        if (gameManager.GetEditMode() != GameManager.EditMode.Add &&
            gameManager.GetEditMode() != GameManager.EditMode.Remove)
            return;

        if (hit == null) return;

        if (gameManager.selectedCharacter.TryGetComponent(out WorkerData worker) &&
            hit.TryGetComponent(out FarmTile tile))
        {
            WorkerInteraction wi = worker.GetComponent<WorkerInteraction>();

            if (wi.startTile == null)
                wi.startTile = tile.gameObject;
            else
            {
                wi.endTile = tile.gameObject;
                wi.shownTiles.Clear();
            }
        }
    }

    void InteractNormalMode(Collider2D hit)
    {
        if (gameManager.GetEditMode() != GameManager.EditMode.Normal) return;
        if (hit == null) return;

        if (hit.TryGetComponent(out FarmTile tile))
        {
            WorkerInteraction wi = gameManager.selectedCharacter.GetComponent<WorkerInteraction>();

            if (!wi.selectedTiles.Contains(tile.gameObject))
                wi.Add(tile.gameObject);
            else
                wi.Remove(tile.gameObject);
        }
    }
}
