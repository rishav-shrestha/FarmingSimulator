using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class StorageInputController : MonoBehaviour
{
    public float tapThreshold = 0.2f;
    public float dragThreshold = 20f;

    private bool _isDragging;
    private Vector2 _startPos;
    private float _startTime;
    private StorageUnit _hoveredStorageUnit;
    private Plot _hoveredPlot;
    private PlotManager _plotManager;

    
    void Start()
    {
        _plotManager = GetComponent<PlotManager>();
    }
    void Update()
    {
        HandleMouse();
    }
    
    
    void HandleMouse()
    {
        if (IsPointerOverUI()) return;

        HandleMousePress();
        HandleMouseDrag();
        UpdateHoveredObjects();
        HandleMouseRelease();
       
       
    }




    bool IsPointerOverUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            ClearHoveredObjects();
            return true;
        }
        return false;
    }

    void ClearHoveredObjects()
    {
        if (_hoveredStorageUnit != null)
        {
            _hoveredStorageUnit.hovered = false;
            _hoveredStorageUnit = null;
        }
        if (_hoveredPlot != null)
        {
            _hoveredPlot.hovered = false;
            _hoveredStorageUnit = null;
        }
    }
 
    void HandleMousePress()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _startPos = Mouse.current.position.ReadValue();
            _startTime = Time.time;
            _isDragging = false;
        }
    }

    void HandleMouseDrag()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            float distance = Vector2.Distance(_startPos, Mouse.current.position.ReadValue());
            if (distance > dragThreshold) _isDragging = true;
        }
    }
 
    void UpdateHoveredObjects()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        worldPos.z = 0;

        ClearHoveredObjects();
        Find(worldPos); 
    }

    void HandleMouseRelease()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            float duration = Time.time - _startTime;
            if (!_isDragging && duration <= tapThreshold)
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                worldPos.z = 0;
                if (_plotManager.currentMode == PlotManager.GameMode.Play) TryInteractPlayMode(worldPos); 
                else if(_plotManager.currentMode == PlotManager.GameMode.Edit) TryInteractEditMode(worldPos);
            }
        }
    }

    void Find(Vector3 worldpos)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldpos);
        HoverStorageUnit(hit);
        if (!HoverStorageUnit(hit)) HoverPlot(hit);  
    }

    bool HoverStorageUnit(Collider2D hit)
    {
        if (hit != null && hit.TryGetComponent(out StorageUnit unit))
        {
            _hoveredStorageUnit = unit;
            unit.hovered = true;
            return true;
        }
        return false;
    }

    void HoverPlot(Collider2D hit)
    {
        if (hit != null && hit.TryGetComponent(out Plot plot))
        {
            plot.hovered = true;
            _hoveredPlot = plot;
        }
    }
    void TryInteractPlayMode(Vector3 worldPos)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        SelectUnit(hit);
        
    }
    public void SelectUnit(Collider2D hit)
    {
        if (hit != null)
        {
            if (hit.TryGetComponent(out StorageUnit unit))
            {
                _plotManager.selectedUnit=unit;
            }
        }
    }

    public void TryInteractEditMode(Vector3 worldPos)
    {
        
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
    }

  
}
