using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    
    public static UIController Instance;
    public Image cropImage;
    public Inventory inventory;

    [Header("Buttons")]
    
    public Button plantbutton;
    public Button harvestbutton;
    public Button waterbutton;
    public Button pausebutton;
    public Button playbutton;
    public Button exiteditmodeButton;
    public Button graphicsbutton;
    [Header("Sprites")]
    
    public Sprite plantSelectedSprite;
    public Sprite harvestSelectedSprite;
    public Sprite waterSelectedSprite;
    public Sprite plantUnselectedSprite;
    public Sprite harvestUnselectedSprite;
    public Sprite waterUnselectedSprite;
    public Sprite lowGraphicsSprite;
    public Sprite highGraphicsSprite;
    [Header("UI Elements")]
    
    public GameObject playModeUI;
    public GameObject editModeUI;
    public GameObject gameoverscreen;
    public GameObject workerUIAdditive;
    public TextMeshProUGUI coinsDisplay;
    public TextMeshProUGUI seedAmountDisplay;
    public TextMeshProUGUI cropAmountDisplay;
    [Header("GameComponents")] 
    
    private GameManager _gameManager;
    private Inventory _inventory;

    [Header("UI")] public GameObject mapUI;
    public GameObject storageUI;
    public GameObject farmUI;
    public GameObject[] ui;
    

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _inventory = _gameManager.inventory;
        ui = new GameObject[3] {mapUI, storageUI, farmUI};
    }
    public void Update()
    {
        UpdateUI();
    }
    public void UpdateUI()
    {
        SetGraphics();
        UpdateCropImage();
        UpdateTool();
        UpdateSeedAndCropAmount();
        UpdateCoins();
        if (_gameManager.GetGameMode() == GameManager.GameMode.Play)
        {
            if(_gameManager.selectedCharacter.CompareTag("Worker"))
            {
                if (workerUIAdditive.activeSelf == false) workerUIAdditive.SetActive(true);
            }
            else if(_gameManager.selectedCharacter.CompareTag("Player"))
            {
                if (workerUIAdditive.activeSelf == true) workerUIAdditive.SetActive(false);
            }   
        }
        else if (_gameManager.GetGameMode() == GameManager.GameMode.Edit)
        {
            if (workerUIAdditive.activeSelf == true) workerUIAdditive.SetActive(false);
        }
    }
    public void UpdateTool()
    {
        
        if(_gameManager.selectedCharacter.CompareTag("Player")
           &&inventory.currentTool == Inventory.Tool.Planting) SelectPlantTool();
        else if(_gameManager.selectedCharacter.CompareTag("Player")
                &&inventory.currentTool == Inventory.Tool.Harvesting) SelectHarvestTool();
        else if(_gameManager.selectedCharacter.CompareTag("Player")
                &&inventory.currentTool == Inventory.Tool.Watering) SelectWaterTool();
        else if(_gameManager.selectedCharacter.CompareTag("Player")&&inventory.currentTool==Inventory.Tool.None) SelectNoneTool();
        else if(_gameManager.selectedCharacter.CompareTag("Worker")
                &&_gameManager.selectedCharacter.GetComponent<WorkerInteraction>().assignedWork == Inventory.Tool.Planting) SelectPlantTool();
        else if(_gameManager.selectedCharacter.CompareTag("Worker")
                &&_gameManager.selectedCharacter.GetComponent<WorkerInteraction>().assignedWork == Inventory.Tool.Harvesting) SelectHarvestTool();
        else if(_gameManager.selectedCharacter.CompareTag("Worker")
                &&_gameManager.selectedCharacter.GetComponent<WorkerInteraction>().assignedWork == Inventory.Tool.Watering) SelectWaterTool();
        else if(_gameManager.selectedCharacter.CompareTag("Worker")
                &&_gameManager.selectedCharacter.GetComponent<WorkerInteraction>().assignedWork == Inventory.Tool.None) SelectNoneTool();
    }
    public void UpdateCropImage()
    {
            if(_gameManager.selectedCharacter.CompareTag("Player")&&
               cropImage.sprite != inventory.selectedCrop.icon) cropImage.sprite = _inventory.selectedCrop.icon;  
            else if(_gameManager.selectedCharacter.CompareTag("Worker")&&
                    cropImage.sprite != 
                    _gameManager.selectedCharacter.GetComponent<WorkerInteraction>().selectedcrop.icon)  
                cropImage.sprite = _gameManager.selectedCharacter.GetComponent<WorkerInteraction>().selectedcrop.icon;
    }

    public void UpdateSeedAndCropAmount()
    {

        if (_gameManager.selectedCharacter.CompareTag("Player"))
        {
            seedAmountDisplay.SetText(OptimizeInt(_inventory.GetSeedAmount(_inventory.selectedCrop)));
            cropAmountDisplay.SetText(OptimizeInt(_inventory.GetCropAmount(_inventory.selectedCrop)));   
        }
        else if (_gameManager.selectedCharacter.CompareTag("Worker"))
        {
            seedAmountDisplay.SetText(OptimizeInt(_inventory.
                GetSeedAmount(_gameManager.selectedCharacter.GetComponent<WorkerInteraction>().selectedcrop)));
            cropAmountDisplay.SetText(OptimizeInt(_inventory.
                GetCropAmount(_gameManager.selectedCharacter.GetComponent<WorkerInteraction>().selectedcrop)));
        }
            
        
    }
    public void UpdateCoins()
    {
        coinsDisplay.SetText(OptimizeInt(_inventory.coins));
    }

    public string OptimizeInt(int seedAmount)
    {
        if (seedAmount >= 1000000f)
            return (seedAmount / 1000000f).ToString("0.#") + "M"; 
        if (seedAmount >= 1000f)
            return (seedAmount / 1000f).ToString("0.#") + "k"; 
        return seedAmount.ToString("0"); // less than 1000, show normally
    }

   public void SelectPlantTool()
    {
        if (_gameManager.selectedCharacter.CompareTag("Player")) {
            _inventory.currentTool = Inventory.Tool.Planting;
            plantbutton.image.sprite = plantSelectedSprite;
            harvestbutton.image.sprite = harvestUnselectedSprite;
            waterbutton.image.sprite = waterUnselectedSprite; 
        }
        else if (_gameManager.selectedCharacter.CompareTag("Worker")) {
            _gameManager.selectedCharacter.GetComponent<WorkerInteraction>().assignedWork = Inventory.Tool.Planting;
            plantbutton.image.sprite = plantSelectedSprite;
            harvestbutton.image.sprite = harvestUnselectedSprite;
            waterbutton.image.sprite = waterUnselectedSprite;
        }
    }
    public void SelectHarvestTool()
    {
        if (_gameManager.selectedCharacter.CompareTag("Player")) {
            _inventory.currentTool = Inventory.Tool.Harvesting;
            plantbutton.image.sprite = plantUnselectedSprite;
            harvestbutton.image.sprite = harvestSelectedSprite;
            waterbutton.image.sprite = waterUnselectedSprite;
        }
        else if (_gameManager.selectedCharacter.CompareTag("Worker")) {
            _gameManager.selectedCharacter.GetComponent<WorkerInteraction>().assignedWork = Inventory.Tool.Harvesting;
            plantbutton.image.sprite = plantUnselectedSprite;
            harvestbutton.image.sprite = harvestSelectedSprite;
            waterbutton.image.sprite = waterUnselectedSprite;
        }
    }
    public void SelectWaterTool()
    {
       if (_gameManager.selectedCharacter.CompareTag("Player")) {
           _inventory.currentTool = Inventory.Tool.Watering;
            plantbutton.image.sprite = plantUnselectedSprite;
            harvestbutton.image.sprite = harvestUnselectedSprite;
            waterbutton.image.sprite = waterSelectedSprite;
        }
        else if (_gameManager.selectedCharacter.CompareTag("Worker")) {
            _gameManager.selectedCharacter.GetComponent<WorkerInteraction>().assignedWork = Inventory.Tool.Watering;
            plantbutton.image.sprite = plantUnselectedSprite;
            harvestbutton.image.sprite = harvestUnselectedSprite;
            waterbutton.image.sprite = waterSelectedSprite;
        }
    }

    public void SelectNoneTool()
    {
        plantbutton.image.sprite = plantUnselectedSprite;
        harvestbutton.image.sprite = harvestUnselectedSprite;
        waterbutton.image.sprite = waterUnselectedSprite;
    }

    public void UnPauseGame()
    {
        if(_gameManager.GetPreviousMode()==GameManager.GameMode.Edit) editModeUI.SetActive(true);
        if(_gameManager.GetPreviousMode()==GameManager.GameMode.Play) playModeUI.SetActive(true);
    }

    public void ExpandFarm()
    {
        if (inventory.SpendCoins(3000 * _gameManager.farmCostMultiplier))
        {
            _gameManager.tileManager.GetComponent<TileManager>().ExpandFarm(1, 1);
            _gameManager.farmCostMultiplier++;
            _gameManager.passiveExpensesCost+=700* _gameManager.farmCostMultiplier;
        }
    }

    public void SetGraphics()
    {
        if (PlayerPrefs.HasKey("GraphicsMode") == false)
        {
            PlayerPrefs.SetString("GraphicsMode", _gameManager.graphicsMode.ToString());  
        }
        if (PlayerPrefs.GetString("GraphicsMode") == "Low")
        {
            graphicsbutton.image.sprite = lowGraphicsSprite;
            _gameManager.graphicsMode = GameManager.GraphicsMode.Low;
        }
        else 
        {
            graphicsbutton.image.sprite = highGraphicsSprite;
            _gameManager.graphicsMode = GameManager.GraphicsMode.High;
        }

       
        
    }
    public void CycleGraphics()
    {
        _gameManager.CycleGraphicsMode();
        if (_gameManager.graphicsMode == GameManager.GraphicsMode.High)
        {
           PlayerPrefs.SetString("GraphicsMode", "High"); 
            graphicsbutton.image.sprite = highGraphicsSprite;
        }
        else
        {
            graphicsbutton.image.sprite = lowGraphicsSprite;
            PlayerPrefs.SetString("GraphicsMode", "Low"); 
        }
    }
    public void AddWorker()
    {
        if (inventory.SpendCoins(1000))
        {
            Instantiate(_gameManager.workerPrefab, _gameManager.player.transform.position, Quaternion.identity);
            _gameManager.farmCostMultiplier++;
            _gameManager.passiveExpensesCost+=100;
        }
    }
    public void ExitGame()  
    {
        _gameManager.ExitGame();
    }
}
