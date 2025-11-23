using UnityEngine;


public class WorkerMovement : MonoBehaviour
{
    private GameManager _gameManager;
    public float speed = 3f;
    public Vector2 currentPosition;
    public Vector2 targetPosition;
    public Vector2 idlePosition;
    
    private WorkerState _workerState;
    private WorkerInteraction _workerInteraction;

    void Start()
    {
        _workerInteraction = GetComponent<WorkerInteraction>();
        _workerState = GetComponent<WorkerState>();
        _gameManager= GameObject.Find("GameManager").GetComponent<GameManager>();
        currentPosition = transform.position;
        idlePosition = transform.position;
    }
    void Update()
    {

        switch (_workerState.currentState)
        {
            case WorkerState.State.Idle:
                
                break;
            case WorkerState.State.GoingtoTarget :
                break;
            case WorkerState.State.Interacting :
                break;
            case WorkerState.State.ReturningToIdle :
                break;
        }
    }

    public void FindTarget()
    {
        
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
    
}
