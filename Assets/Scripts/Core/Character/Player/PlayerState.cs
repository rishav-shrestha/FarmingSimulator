using UnityEngine;

public class PlayerState : MonoBehaviour
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
