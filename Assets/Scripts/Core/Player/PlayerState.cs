using UnityEngine;

public class PlayerState : MonoBehaviour
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
