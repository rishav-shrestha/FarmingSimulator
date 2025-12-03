using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Camera gameCam;
    public Camera storageCam;
    public Camera mapCam;
    public Inventory inventory;
    public GameObject tileManager;
    public UIController uIcontroller;
    public GameObject player;
    public GameObject[] workers;
    public GameObject selectedCharacter;
    public GameMode currentMode;
    private GameMode _previousMode;
    public WorkerEditMode workerEditMode;
    public GraphicsMode graphicsMode;
    public Scene currentScene;
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
        gameCam.enabled = true;
        storageCam.enabled = false;
        mapCam.enabled = false;
        tileManager= GameObject.FindGameObjectWithTag("TileManager");
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        player = GameObject.FindGameObjectWithTag("Player");
        workers = GameObject.FindGameObjectsWithTag("Worker");
        uIcontroller = GetComponent<UIController>();
        SelectCharacter(player);
        SetGameMode(GameMode.Farm);
        SetNormalEditMode();
        graphicsMode = GraphicsMode.High;
        
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
    public void SetGameMode(GameMode mode)
    {
        currentMode = mode;
    }

    public GameMode GetPreviousMode()
    {
        return _previousMode;
    }
    
    public void SetAddEditMode()
    {
        if(GetEditMode()!=WorkerEditMode.Add) selectedCharacter.GetComponent<WorkerInteraction>().startTile = null;
        if(GetEditMode()!=WorkerEditMode.Add) selectedCharacter.GetComponent<WorkerInteraction>().endTile = null;
        workerEditMode = WorkerEditMode.Add;
    }

    public void SetRemoveEditMode()
    {
        if(GetEditMode()!=WorkerEditMode.Remove) selectedCharacter.GetComponent<WorkerInteraction>().startTile = null;
        if(GetEditMode()!=WorkerEditMode.Remove) selectedCharacter.GetComponent<WorkerInteraction>().endTile = null;
        workerEditMode = WorkerEditMode.Remove;
    }
    public void SetNormalEditMode()
    {
        if (selectedCharacter.CompareTag("Worker"))
        {
            selectedCharacter.GetComponent<WorkerInteraction>().startTile = null;
            selectedCharacter.GetComponent<WorkerInteraction>().endTile = null;   
        }
        workerEditMode = WorkerEditMode.Normal;
    }
    public void ClearSelectedTiles()
    {
        selectedCharacter.GetComponent<WorkerInteraction>().ClearTiles();
        selectedCharacter.GetComponent<WorkerInteraction>().startTile = null;
        selectedCharacter.GetComponent<WorkerInteraction>().endTile = null;
    }
    public void CycleGameMode()
    {
        switch (currentMode)
        {
            case GameMode.Farm:
                SetGameMode(GameMode.WorkerEdit);
                break;
            case GameMode.WorkerEdit:
                SetGameMode(GameMode.Farm);
                break;
        }
    }

    public void PauseGame()
    {
        _previousMode = currentMode;
        SetGameMode(GameMode.Pause);
    }

    public void UnpauseGame()
    {
        SetGameMode(_previousMode);
        GetComponent<UIController>().UnPauseGame();
    }
    public WorkerEditMode GetEditMode()
    {
        return workerEditMode;
    }
    public GameMode GetGameMode()
    {
        return currentMode;
    }
    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public enum GameMode
    {
        Farm,
        Storage,
        StorageEdit,
        WorkerEdit,
        Pause,
        Map
    }
    public enum WorkerEditMode
    {
        Add,
        Remove,
        Normal
    }
    public enum GraphicsMode
    {
        Low,
        High
    }
}
