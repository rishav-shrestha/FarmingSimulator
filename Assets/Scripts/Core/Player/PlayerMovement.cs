using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Vector2 targetPosition;
    public Vector2 idlePosition;
    public Vector2 currentPosition;
    public float speed = 5f;
    private PlayerState playerState;
    private PlayerInteraction playerInteraction;
    void Start()
    {
        playerInteraction = GetComponent<PlayerInteraction>();  
        playerState = GetComponent<PlayerState>();
        idlePosition = transform.position;
        currentPosition = transform.position;
    }
    void Update()
    {
            currentPosition = transform.position;
 
        if(IsAtTarget())
        {
            if(playerInteraction.currentSelectedTile == null)
            {
                playerState.currentState = PlayerState.State.returningToIdle;
            }   
        }
        if(IsAtIdle())
        {
        playerState.currentState = PlayerState.State.Idle;
        }
        switch (playerState.currentState)
        {
            case PlayerState.State.goingtoTarget:
                MoveToTarget();
                break;
            case PlayerState.State.returningToIdle:
                ReturnToIdle();
                if(playerInteraction.currentSelectedTile != null)
                {
                    playerState.currentState = PlayerState.State.goingtoTarget;
                    SetTargetPosition(playerInteraction.currentSelectedTile.transform.position);
                }
                break;
            case PlayerState.State.Idle:
                if (playerInteraction.currentSelectedTile != null)
                {
                    playerState.currentState = PlayerState.State.goingtoTarget;
                    SetTargetPosition(playerInteraction.currentSelectedTile.transform.position);
                }
                break;
            case PlayerState.State.Interacting:      
                if(playerInteraction.currentSelectedTile != null)
                {
                    playerState.currentState = PlayerState.State.goingtoTarget;
                    SetTargetPosition(playerInteraction.currentSelectedTile.transform.position);
                }
                else
                {
                    playerState.currentState = PlayerState.State.returningToIdle;
                }
                playerInteraction.interactTile(playerInteraction.currentSelectedTile);
                break;
        }
    }
    
    public void SetTargetPosition(Vector2 target)
    {
        targetPosition = target;
        playerState.currentState = PlayerState.State.goingtoTarget;
    }
    public bool IsAtTarget()
    {
        return Vector2.Distance(currentPosition, targetPosition) < 0.1f;
    }   
    public bool IsAtIdle()
    {
        if(playerState.currentState != PlayerState.State.returningToIdle)
        {
            return false;
        }
        return Vector2.Distance(currentPosition, idlePosition) < 0.1f;
    }
    public void MoveToTarget()
    {
        transform.position = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);
        if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
        {
            playerState.currentState = PlayerState.State.Interacting;
        }
    }
    public void ReturnToIdle()
    {
        transform.position = Vector2.MoveTowards(currentPosition, idlePosition, speed * Time.deltaTime);
        if (Vector2.Distance(currentPosition, idlePosition) < 0.1f)
        {
            playerState.currentState = PlayerState.State.Idle;
        }
    }
}
