using UnityEngine;


public class WorkerMovement : MonoBehaviour
{
    private GameManager _gameManager;
    public float speed = 3f;
    public Vector2 currentPosition;
    public Vector2 targetPosition;
    public Vector2 idlePosition;
    
    private Vector3 lastPosition;
    private bool facingRight = true;
    private Animator _animator;
    
    private WorkerState _workerState;
    private WorkerInteraction _workerInteraction;

    void Start()
    {
        _animator = GetComponent<Animator>();
        lastPosition = transform.position;
        _workerInteraction = GetComponent<WorkerInteraction>();
        _workerState = GetComponent<WorkerState>();
        _gameManager= GameObject.Find("GameManager").GetComponent<GameManager>();
        currentPosition = transform.position;
        idlePosition = transform.position;
    }
    void Update()
    {
        if(_gameManager.GetGameMode()==GameManager.GameMode.Pause) return;
        switch (_workerState.currentState)
        {
            case WorkerState.State.Idle:
                FindTarget();
                if(_workerInteraction.assignedWork==Inventory.Tool.Planting && !GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>()
                       .HasSeeds(_workerInteraction.selectedcrop))
                    _workerInteraction.currentSelectedTile = null;
                if (_workerInteraction.currentSelectedTile != null)
                {
                    SetTargetPosition(_workerInteraction.currentSelectedTile.transform.position);
                    _workerState.currentState = WorkerState.State.GoingtoTarget;
                }
                break;
            case WorkerState.State.GoingtoTarget :
                MoveTo(targetPosition);
                if (IsAtTarget())
                {
                    _workerState.currentState = WorkerState.State.Interacting;
                }
                break;
            case WorkerState.State.Interacting :
                _workerInteraction.Interact(_workerInteraction.currentSelectedTile);
                FindTarget();
                if (_workerInteraction.currentSelectedTile != null)
                {
                    SetTargetPosition(_workerInteraction.currentSelectedTile.transform.position);
                    _workerState.currentState = WorkerState.State.GoingtoTarget;
                }
                else
                {
                    _workerState.currentState = WorkerState.State.ReturningToIdle;
                }
                
                break;
            case WorkerState.State.ReturningToIdle :
                ReturnToIdle();
                if (IsAtIdle())
                {
                    _workerState.currentState = WorkerState.State.Idle;
                }
                FindTarget();
                if (_workerInteraction.currentSelectedTile != null)
                {
                    SetTargetPosition(_workerInteraction.currentSelectedTile.transform.position);
                    _workerState.currentState = WorkerState.State.GoingtoTarget;
                }
                break;
        }
        if (_workerState.currentState == WorkerState.State.Idle)
        {
            _animator.SetBool("Moving", false);
        }
        else
        {
            _animator.SetBool("Moving", true);
        }

        Vector3 delta = transform.position - lastPosition;

        if (delta.x > 0 && !facingRight)
            Flip();
        else if (delta.x < 0 && facingRight)
            Flip();

        lastPosition = transform.position; 
        currentPosition = transform.position;
    }

    public void FindTarget()
    {
        if (_workerInteraction.currentSelectedTile == null)
        {
            if (_workerInteraction.assignedWork==Inventory.Tool.Planting&& GameObject
                    .FindGameObjectWithTag("Inventory").GetComponent<Inventory>()
                    .HasSeeds(_workerInteraction.selectedcrop))
            {
                FarmTile w = _workerInteraction.GetNearestTile(FarmTile.TileState.Empty);
                if (w != null) _workerInteraction.currentSelectedTile=w.gameObject;
            }
            else if (_workerInteraction.assignedWork == Inventory.Tool.Harvesting)
            {
                FarmTile w = _workerInteraction.GetNearestTile(FarmTile.TileState.FullyGrown);
                if (w == null) w = _workerInteraction.GetNearestTile(FarmTile.TileState.Dead);
                if (w != null) _workerInteraction.currentSelectedTile=w.gameObject;
            }
            else if (_workerInteraction.assignedWork == Inventory.Tool.Watering)
            {
                FarmTile w = _workerInteraction.GetNearestTile(FarmTile.TileState.RequiresWater);
                if (w != null) _workerInteraction.currentSelectedTile=w.gameObject;
            }
        }
    }
    public void MoveTo(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
    public void ReturnToIdle()
    {
        transform.position = Vector2.MoveTowards(transform.position, idlePosition, speed * Time.deltaTime);
    }
    public void SetIdlePosition(Vector2 idle)
    {
        idlePosition = idle;
    }
    public void SetTargetPosition(Vector2 target)
    {
        targetPosition = target;
    }
    public bool IsAtTarget()
    {
        return Vector2.Distance(currentPosition, targetPosition) < 0.1f;
    }
    public bool IsAtIdle()
    {
        return Vector2.Distance(currentPosition, idlePosition) < 0.1f;
    }
    public void SetCurrentPosition(Vector2 position)
    {
        currentPosition = position;
    }
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
