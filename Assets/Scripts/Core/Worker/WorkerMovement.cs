using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class WorkerMovement : MonoBehaviour
{
    private GameManager _gameManager;

    void Start()
    {
        _gameManager= GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    void Update()
    {
            if (_gameManager.selectedCharacter == gameObject)
            {
                transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y),
                    new Vector2(0, 0), Time.deltaTime * (1));
            }
    }
}
