using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class NewInputController : MonoBehaviour
{
    public float tapThreshold = 0.2f;
    public float dragThreshold = 20f;

    private bool _isDragging;
    private Vector2 _startPos;
    private float _startTime;
    private FarmTile _hoveredTile;
    private GameObject _hoveredCharacter;
    private bool _isPlayer;
    private GameManager _gameManager;
    
    void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    void Update()
    {
        HandleMouse();

    }

    void HandleMouse()
    {
        
        
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            if (_hoveredTile != null)
            {
                _hoveredTile.isHovered = false;
                _hoveredTile = null;
            }
            if (_hoveredCharacter != null)
            {
                if (_isPlayer)
                {
                    _hoveredCharacter.GetComponent<PlayerData>().hovered = false;
                }
                else
                {
                    _hoveredCharacter.GetComponent<WorkerData>().hovered = false; 
                }
                _isPlayer = false;
                _hoveredCharacter = null;
            }
            return;
        }


        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _startPos = Mouse.current.position.ReadValue();
            _startTime = Time.time;
            _isDragging = false;
        }
            if (Mouse.current.leftButton.isPressed)
        {
            float distance = Vector2.Distance(_startPos, Mouse.current.position.ReadValue());
            if (distance > dragThreshold) _isDragging = true;
        }

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        worldPos.z = 0;
        if(_hoveredTile != null)
        {
            _hoveredTile.isHovered = false;
            _hoveredTile = null;
        }

        if (_hoveredCharacter != null)
        {
            if (_isPlayer)
            {
                _hoveredCharacter.GetComponent<PlayerData>().hovered = false;
            }
            else
            {
                _hoveredCharacter.GetComponent<WorkerData>().hovered = false; 
            }
            _isPlayer = false;
            _hoveredCharacter = null;
        }
        Find(worldPos); 
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            float duration = Time.time - _startTime;
            if (!_isDragging && duration <= tapThreshold)
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
            _hoveredCharacter = player.gameObject;
            _isPlayer = true;
            player.hovered = true;
        }
        else if (hit != null&&hit.TryGetComponent(out WorkerData worker))
        {
            _hoveredCharacter = worker.gameObject;
            worker.hovered = true;
        }
        else if (hit != null && hit.TryGetComponent(out FarmTile tile))
        {
            tile.isHovered = true;
            _hoveredTile = tile;
        }
    }
    void TryInteract(Vector3 worldPos)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
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
        
        if (_gameManager.selectedCharacter.TryGetComponent(out PlayerData data) 
            && hit != null && hit.TryGetComponent(out FarmTile tile))
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<PlayerInteraction>().AddTile(tile.gameObject);
            }

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
            }
 
            
        }
        }
        
    }