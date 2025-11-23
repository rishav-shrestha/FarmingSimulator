using UnityEngine;

public class WorkerState : MonoBehaviour
{
    public enum State
    {
        Idle,
        goingtoTarget,
        Interacting,
        returningToIdle
    }
    
    public State currentState = State.Idle;
}
