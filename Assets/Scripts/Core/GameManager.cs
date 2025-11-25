using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Inventory inventory;
    public GameObject tileManager;
    public GameObject player;
    public GameObject[] workers;
    public GameObject selectedCharacter;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        tileManager= GameObject.FindGameObjectWithTag("TileManager");
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        player = GameObject.FindGameObjectWithTag("Player");
        workers = GameObject.FindGameObjectsWithTag("Worker");
        SelectCharacter(player);
        
    }

    public void CycleWorkerCrop()
    {
        if (selectedCharacter.CompareTag("Worker"))
        {
            selectedCharacter.GetComponent<WorkerInteraction>().CycleCrop();
        }
    }
    public void SelectCharacter(GameObject character)
    {
        selectedCharacter = character;
    }
    
    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
