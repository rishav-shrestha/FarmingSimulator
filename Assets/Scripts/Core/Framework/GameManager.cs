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
    public GameMode currentMode;
    private GameMode _previousMode;
    public EditMode editMode;
    public GraphicsMode graphicsMode;
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
        SetGameMode(GameMode.Play);
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
        if(GetEditMode()!=EditMode.Add) selectedCharacter.GetComponent<WorkerInteraction>().startTile = null;
        if(GetEditMode()!=EditMode.Add) selectedCharacter.GetComponent<WorkerInteraction>().endTile = null;
        editMode = EditMode.Add;
    }

    public void SetRemoveEditMode()
    {
        if(GetEditMode()!=EditMode.Remove) selectedCharacter.GetComponent<WorkerInteraction>().startTile = null;
        if(GetEditMode()!=EditMode.Remove) selectedCharacter.GetComponent<WorkerInteraction>().endTile = null;
        editMode = EditMode.Remove;
    }
    public void SetNormalEditMode()
    {
        if (selectedCharacter.CompareTag("Worker"))
        {
            selectedCharacter.GetComponent<WorkerInteraction>().startTile = null;
            selectedCharacter.GetComponent<WorkerInteraction>().endTile = null;   
        }
        editMode = EditMode.Normal;
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
            case GameMode.Play:
                SetGameMode(GameMode.Edit);
                break;
            case GameMode.Edit:
                SetGameMode(GameMode.Play);
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
    public EditMode GetEditMode()
    {
        return editMode;
    }
    public GameMode GetGameMode()
    {
        return currentMode;
    }

    public void HideGameLayer()
    {
        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("Gameplay"));
    }

    public void showGameLayer()
    {
        Camera.main.cullingMask |= (1 << LayerMask.NameToLayer("Gameplay"));
    }
    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public enum GameMode
    {
        Play,
        Edit,
        Pause,
        Inactive
    }
    public enum EditMode
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
