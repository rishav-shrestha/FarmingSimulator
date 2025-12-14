using UnityEngine;

public class WorkerState : MonoBehaviour
{
    public enum State
    {
        Idle,
        GoingtoTarget,
        Interacting,
        ReturningToIdle
    }
    
    public State currentState = State.Idle;
}
