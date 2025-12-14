using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Camera[] cams;
    public static GameManager Instance;
    public Inventory inventory;
    public GameObject tileManager;
    public PlotManager plotManager;
    public MapController mapController;
    public UIController uiController;
    public GameObject player;
    public GameObject[] workers;
    public GameObject workerPrefab;
    public GameObject selectedCharacter;
    public GameMode currentMode;
    private GameMode _previousMode;
    public EditMode editMode;
    public GraphicsMode graphicsMode;
    public float passiveExpensesTimer;
    public int passiveExpensesCost;
    public int farmCostMultiplier=1;
    public int bar;
    private void Awake()
    {
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
        SelectCharacter(player);
        SetGameMode(GameMode.Play);
        SetNormalEditMode();
        graphicsMode = GraphicsMode.High;
        Intialize();
    }

    public void Intialize()
    {
        AudioManager.Instance.Initialize(this);
    }
    public void Update()
    {
        passiveExpensesTimer+=Time.deltaTime;
        if (passiveExpensesTimer >= 200)
        {
            inventory.coins-=(passiveExpensesCost);
            passiveExpensesTimer = 0;
        }

        
    }
 

    public void CycleWorkerCrop()
    {
        if (selectedCharacter.CompareTag("Worker"))
        {
            selectedCharacter.GetComponent<WorkerInteraction>().CycleCrop();
        }
    }
    public void SelectCharacter(GameObject character) { selectedCharacter = character; }
    public void SetGameMode(GameMode mode) { currentMode = mode; }

    public GameMode GetPreviousMode() { return _previousMode; }

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
    public void CycleGraphicsMode() { graphicsMode = graphicsMode == GraphicsMode.High ? GraphicsMode.Low : GraphicsMode.High; }

    public void SetEditMode(EditMode editMode)
    {
        if(GetEditMode()!=EditMode.Add) selectedCharacter.GetComponent<WorkerInteraction>().startTile = null;
        if(GetEditMode()!=EditMode.Add) selectedCharacter.GetComponent<WorkerInteraction>().endTile = null;
        this.editMode=editMode;
    }
    public void SetAddEditMode() => SetEditMode(EditMode.Add);
    public void SetRemoveEditMode() => SetEditMode(EditMode.Remove);
    public void SetNormalEditMode() => SetEditMode(EditMode.Normal);
    
    public void ClearSelectedTiles() {
        selectedCharacter.GetComponent<WorkerInteraction>().ClearTiles();
        selectedCharacter.GetComponent<WorkerInteraction>().startTile = null;
        selectedCharacter.GetComponent<WorkerInteraction>().endTile = null; }
    public void CycleGameMode() {
        switch (currentMode)
        {
            case GameMode.Play:
                SetGameMode(GameMode.Edit);
                break;
            case GameMode.Edit:
                SetGameMode(GameMode.Play);
                break;
        } }

    public void PauseGame() {
        _previousMode = currentMode;
        SetGameMode(GameMode.Pause); }

    public void UnpauseGame() { 
        SetGameMode(_previousMode); 
        GetComponent<UIController>().UnPauseGame(); }
    public EditMode GetEditMode() { return editMode; }
    public GameMode GetGameMode() { return currentMode; }
    public void RestartGame() { SceneManager.LoadScene("Game"); }
    public void ExitGame() { SceneManager.LoadScene("MainMenu"); }
    
    public enum GameMode { Play, Edit, Pause, Map }
    public enum EditMode { Add, Remove, Normal }
    public enum StorageEditMode { Relocate, Add, Sell, Remove }
    public enum GraphicsMode { Low, High }
}
