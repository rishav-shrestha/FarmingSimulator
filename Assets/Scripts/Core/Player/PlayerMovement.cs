using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Postions")]
    public Vector2 targetPosition;
    public Vector2 idlePosition;
    public Vector2 currentPosition;

    [Header("Movement Settings")]
    public float speed = 1f;
    private bool facingRight = true;
    private Vector3 lastPosition;

    private GameManager _gameManager;
    private PlayerState _playerState;
    private PlayerInteraction _playerInteraction;
    private Animator _animator;
    void Start()
    {

        // Initialize components
        _animator = GetComponent<Animator>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _playerInteraction = GetComponent<PlayerInteraction>();  
        _playerState = GetComponent<PlayerState>();
        // Set idle position to starting position
        idlePosition = transform.position;
        // Set current position to starting position
        currentPosition = transform.position;
        lastPosition = transform.position;
    }
    void Update()
    {
        if(_gameManager.GetGameMode()==GameManager.GameMode.Pause) return;
        // State machine for player movement
        switch (_playerState.currentState)
        {
            // Move towards target position
            case PlayerState.State.GoingtoTarget:
                MoveToTarget();
                break;
            // Move back to idle position
            case PlayerState.State.ReturningToIdle:
                ReturnToIdle();
                // If a tile is selected while returning to idle, go to that tile
                if (_playerInteraction.currentSelectedTile != null)
                {
                    _playerState.currentState = PlayerState.State.GoingtoTarget;
                    SetTargetPosition(_playerInteraction.currentSelectedTile.transform.position);
                }
                break;
            // Check for selected tile to interact with
            case PlayerState.State.Idle:
                // If a tile is selected, go to that tile
                if (_playerInteraction.currentSelectedTile != null)
                {
                    _playerState.currentState = PlayerState.State.GoingtoTarget;
                    SetTargetPosition(_playerInteraction.currentSelectedTile.transform.position);
                }
                break;
            case PlayerState.State.Interacting:
                _playerInteraction.InteractTile(_playerInteraction.currentSelectedTile);
                if (_playerInteraction.currentSelectedTile != null)
                {
                    SetTargetPosition(_playerInteraction.currentSelectedTile.transform.position);
                    _playerState.currentState = PlayerState.State.GoingtoTarget;
                }
                else
                {
                    _playerState.currentState = PlayerState.State.ReturningToIdle;
                }
                break;
        }

        // Update current position
        currentPosition = transform.position;

        // change the state to returning to idle if at target and no tile is selected
        if (IsAtTarget())
        {
            if (_playerInteraction.currentSelectedTile == null)
            {
                _playerState.currentState = PlayerState.State.ReturningToIdle;
            }
        }

        // change the state to idle if at idle position
        if (IsAtIdle())
        {
            _playerState.currentState = PlayerState.State.Idle;
        }

        if (_playerState.currentState == PlayerState.State.Idle)
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
    }

    // Set the target position for movement
    public void SetTargetPosition(Vector2 target)
    {
        targetPosition = target;
    }
    // Check if the player is at the target position
    public bool IsAtTarget()
    {
        return Vector2.Distance(currentPosition, targetPosition) < 0.1f;
    }
    // Check if the player is at the idle position
    public bool IsAtIdle()
    {
        if(_playerState.currentState != PlayerState.State.ReturningToIdle)
        {
            return false;
        }
        return Vector2.Distance(currentPosition, idlePosition) < 0.1f;
    }
    // Move the player towards the target position
    public void MoveToTarget()
    {
        transform.position = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);
        if (IsAtTarget())
        {

            _playerState.currentState = PlayerState.State.Interacting;
        }
    }
    // Move the player back to the idle position
    public void ReturnToIdle()
    {
        transform.position = Vector2.MoveTowards(currentPosition, idlePosition, speed * Time.deltaTime);
        if (Vector2.Distance(currentPosition, idlePosition) < 0.1f)
        {
            _playerState.currentState = PlayerState.State.Idle;
        }
    }
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
