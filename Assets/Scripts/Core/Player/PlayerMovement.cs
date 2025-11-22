using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Postions")]
    public Vector2 targetPosition;
    public Vector2 idlePosition;
    public Vector2 currentPosition;

    [Header("Movement Settings")]
    public float speed = 1f;


    private PlayerState playerState;
    private PlayerInteraction playerInteraction;
    void Start()
    {

        // Initialize components
        playerInteraction = GetComponent<PlayerInteraction>();  
        playerState = GetComponent<PlayerState>();
        // Set idle position to starting position
        idlePosition = transform.position;
        // Set current position to starting position
        currentPosition = transform.position;
    }
    void Update()
    {

        // State machine for player movement
        switch (playerState.currentState)
        {
            // Move towards target position
            case PlayerState.State.goingtoTarget:
                MoveToTarget();
                break;
            // Move back to idle position
            case PlayerState.State.returningToIdle:
                ReturnToIdle();
                // If a tile is selected while returning to idle, go to that tile
                if (playerInteraction.currentSelectedTile != null)
                {
                    playerState.currentState = PlayerState.State.goingtoTarget;
                    SetTargetPosition(playerInteraction.currentSelectedTile.transform.position);
                }
                break;
            // Check for selected tile to interact with
            case PlayerState.State.Idle:
                // If a tile is selected, go to that tile
                if (playerInteraction.currentSelectedTile != null)
                {
                    playerState.currentState = PlayerState.State.goingtoTarget;
                    SetTargetPosition(playerInteraction.currentSelectedTile.transform.position);
                }
                break;
            // Handle interaction with selected tile
            case PlayerState.State.Interacting:
                // Interact with the current selected tile
                playerInteraction.interactTile(playerInteraction.currentSelectedTile);
                Debug.Log("ChangeStatetoInteracting");
                // If another tile is selected, go to that tile
                if (playerInteraction.currentSelectedTile != null)
                {
                    SetTargetPosition(playerInteraction.currentSelectedTile.transform.position);
                    playerState.currentState = PlayerState.State.goingtoTarget;
                }
                // If no tile is selected, return to idle
                else
                {
                    playerState.currentState = PlayerState.State.returningToIdle;
                }
                break;
        }

        // Update current position
        currentPosition = transform.position;

        // change the state to returning to idle if at target and no tile is selected
        if (IsAtTarget())
        {
            if (playerInteraction.currentSelectedTile == null)
            {
                playerState.currentState = PlayerState.State.returningToIdle;
            }
        }

        // change the state to idle if at idle position
        if (IsAtIdle())
        {
            playerState.currentState = PlayerState.State.Idle;
        }
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
        if(playerState.currentState != PlayerState.State.returningToIdle)
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

            playerState.currentState = PlayerState.State.Interacting;
        }
    }
    // Move the player back to the idle position
    public void ReturnToIdle()
    {
        transform.position = Vector2.MoveTowards(currentPosition, idlePosition, speed * Time.deltaTime);
        if (Vector2.Distance(currentPosition, idlePosition) < 0.1f)
        {
            playerState.currentState = PlayerState.State.Idle;
        }
    }
}
