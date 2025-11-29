using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [Header("Sprites")]
    
    public Sprite plantSelectedSprite;
    public Sprite harvestSelectedSprite;
    public Sprite waterSelectedSprite;
    public Sprite plantUnselectedSprite;
    public Sprite harvestUnselectedSprite;
    public Sprite waterUnselectedSprite;
    
    [Header("UI Elements")]
    
    public GameObject playModeUI;
    public GameObject editModeUI;
    public GameObject workerUIAdditive;
    public TextMeshProUGUI coinsDisplay;
    public TextMeshProUGUI seedAmountDisplay;
    [Header("GameComponents")] 
    
    private GameManager _gameManager;
    private Inventory _inventory;

    public UIController(Inventory inventory)
    {
        _inventory = inventory;
    }

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
    }
    public void Update()
    {
        UpdateUI();
    }
    public void UpdateUI()
    {
        UpdateCropImage();
        UpdateTool();
        UpdateSeedAmount();
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

    public void UpdateSeedAmount()
    {
        if (_gameManager.selectedCharacter.CompareTag("Player")) 
            seedAmountDisplay.SetText(OptimizeInt(_inventory.GetSeedAmount(_inventory.selectedCrop)));
        else if (_gameManager.selectedCharacter.CompareTag("Worker"))
            seedAmountDisplay.SetText(OptimizeInt(_inventory.
                GetSeedAmount(_gameManager.selectedCharacter.GetComponent<WorkerInteraction>().selectedcrop)));
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
    public void ExitGame()  
    {
        _gameManager.ExitGame();
    }
}
