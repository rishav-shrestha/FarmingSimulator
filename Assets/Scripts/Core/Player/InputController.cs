using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public float tapThreshold = 0.2f;
    public float dragThreshold = 20f;

    private bool _isDragging;
    private Vector2 _startPos;
    private float _startTime;
    private FarmTile _hoveredTile;
    private GameObject _hoveredCharacter;
    private StorageUnit _hoveredStorageUnit;
    private Plot _hoveredPlot;
    private bool _isPlayer;
    private GameManager _gameManager;
    private CameraController _cameraController;
    private PlotManager _plotManager;
    
    void Start()
    {
        _plotManager = GetComponent<PlotManager>();
        _gameManager = GetComponent<GameManager>();
        _cameraController = GetComponent<CameraController>();
    }
    void Update()
    {
        if (_gameManager.GetGameMode() == GameManager.GameMode.Pause||_gameManager.GetGameMode()==GameManager.GameMode.Map) return;
        
        HandleMouse();
        _cameraController.UpdateCamera();
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
        if (_hoveredTile != null)
        {
            _hoveredTile.isHovered = false;
            _hoveredTile = null;
        }

        if (_hoveredCharacter != null)
        {
            if (_isPlayer)
                _hoveredCharacter.GetComponent<PlayerData>().hovered = false;
            else
                _hoveredCharacter.GetComponent<WorkerData>().hovered = false;

            _hoveredCharacter = null;
            _isPlayer = false;
        }
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
                if (_gameManager.GetGameMode() == GameManager.GameMode.Play) TryInteractPlayMode(worldPos); 
                else if(_gameManager.GetGameMode() == GameManager.GameMode.Edit) TryInteractEditMode(worldPos);
            }
        }
    }

    void Find(Vector3 worldpos)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldpos);
        HoverCharacter(hit);
        if (!HoverCharacter(hit)) HoverTIle(hit);  
        
        HoverStorageUnit(hit);
        if (!HoverStorageUnit(hit)) HoverPlot(hit);  
    }

    bool HoverCharacter(Collider2D hit)
    {
        if (hit != null && hit.TryGetComponent(out PlayerData player))
        {
            _hoveredCharacter = player.gameObject;
            _isPlayer = true;
            player.hovered = true;
            return true;
        }
        if (hit != null&&hit.TryGetComponent(out WorkerData worker))
        {
            _hoveredCharacter = worker.gameObject;
            worker.hovered = true;
            return true;
        }
        return false;
    }

    void HoverTIle(Collider2D hit)
    {
        if (hit != null && hit.TryGetComponent(out FarmTile tile))
        {
            tile.isHovered = true;
            _hoveredTile = tile;
        }
    }
    void TryInteractPlayMode(Vector3 worldPos)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        SelectCharacter(hit);
        SelectTile(hit);
        SelectUnit(hit);
        
    }
    public void SelectCharacter(Collider2D hit)
    {
        if (hit != null)
        {
            if (hit.TryGetComponent(out PlayerData player))
            {
                _gameManager.SelectCharacter(player.gameObject);
            }
            else if (hit.TryGetComponent(out WorkerData worker))
            {
                _gameManager.SelectCharacter(worker.gameObject);
            }
        }
    }

    public void SelectTile(Collider2D hit)
    {
        if (_gameManager.selectedCharacter.TryGetComponent(out PlayerData data) 
            && hit != null && hit.TryGetComponent(out FarmTile tile))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<PlayerInteraction>().AddTile(tile.gameObject);
        }
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
        InteractNormalMode(hit);
        InteractAddRemoveMode(hit);
    }

    private void InteractAddRemoveMode(Collider2D hit)
    {
        if(_gameManager.GetEditMode()==GameManager.EditMode.Remove||_gameManager.GetEditMode()==GameManager.EditMode.Add) 
        {
            if (_gameManager.selectedCharacter.TryGetComponent(out WorkerData workerData)
                && hit != null && hit.TryGetComponent(out FarmTile farmTile))
            {
                GameObject worker = workerData.gameObject;
                if (worker.GetComponent<WorkerInteraction>().startTile == null)
                {
                    worker.GetComponent<WorkerInteraction>().startTile = farmTile.gameObject;  
                }
                else
                {
                    worker.GetComponent<WorkerInteraction>().endTile = farmTile.gameObject;
                    worker.GetComponent<WorkerInteraction>().shownTiles.Clear();
                }
            }  
        }
    }

    public void InteractNormalMode(Collider2D hit)
    {
        if (_gameManager.GetEditMode() == GameManager.EditMode.Normal)
        {
            if (hit != null && hit.TryGetComponent(out FarmTile tile))
            {
                if(!_gameManager.selectedCharacter.GetComponent<WorkerInteraction>().selectedTiles.Contains(tile.gameObject))
                {
                    _gameManager.selectedCharacter.GetComponent<WorkerInteraction>().Add(tile.gameObject);
                }
                else 
                {
                    _gameManager.selectedCharacter.GetComponent<WorkerInteraction>().Remove(tile.gameObject);
                }
            }
        }
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
    }