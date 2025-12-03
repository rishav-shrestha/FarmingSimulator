using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Camera gameCam;
    public Camera storageCam;
    public Camera mapCam;
    public Camera[] cams;
    public static GameManager Instance;
    public Inventory inventory;
    public GameObject tileManager;
    public PlotManager plotManager;
    public MapController mapController;
    public UIController uiController;
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
        plotManager = GetComponent<PlotManager>();
        mapController = GetComponent<MapController>();
        uiController = GetComponent<UIController>();
        cams=new Camera[] {gameCam,storageCam,mapCam};
        SetActiveCamera(gameCam);
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

    public Camera SetActiveCamera(Camera camera)
    {
        foreach (Camera cam in cams)
        {
            cam.enabled = false;
        }
        camera.enabled = true;
        return camera;
    }
    public Camera GetActiveCamera()
    {
        foreach (Camera cam in cams)
        {
            if (cam.enabled) return cam;
        }
        return null;
    }

    public void SetActiveUI(GameObject ui)
    {
        foreach (GameObject u in uiController.ui)
        {
            u.SetActive(false);
        }
        ui.SetActive( true);
    }
    public void CycleGraphicsMode()
    {
        graphicsMode = graphicsMode == GraphicsMode.High ? GraphicsMode.Low : GraphicsMode.High;
    }
    public void CycleEditMode()
    {
        editMode = editMode == EditMode.Add ? EditMode.Remove : EditMode.Add;
    }

    public void CycleLocation()
    {
        if (mapController.location< MapController.Location.Hiring)
        {
            mapController.location++;
        }
        else
        {
            mapController.location = MapController.Location.Farm;
        }
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
        Map
    }
    public enum EditMode
    {
        Add,
        Remove,
        Normal
    }

    public enum StorageEditMode
    {
        Relocate,
        Add,
        Sell,
        Remove
    }
    public enum GraphicsMode
    {
        Low,
        High
    }
}
