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
    
    public GameObject PlayModeUI;
    public GameObject EditModeUI;
    public GameObject workerUIAdditive;
    public TextMeshProUGUI seedAmountDisplay;
    [Header("GameComponents")]
    
    GameManager gameManager;
    Inventory _inventory;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void Start()
    {
        gameManager = GetComponent<GameManager>();
        _inventory = gameManager.inventory;
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
        if (gameManager.GetGameMode() == GameManager.GameMode.Play)
        {
            if(gameManager.selectedCharacter.CompareTag("Worker"))
            {
                if (workerUIAdditive.activeSelf == false) workerUIAdditive.SetActive(true);
            }
            else if(gameManager.selectedCharacter.CompareTag("Player"))
            {
                if (workerUIAdditive.activeSelf == true) workerUIAdditive.SetActive(false);
            }   
        }
        else if (gameManager.GetGameMode() == GameManager.GameMode.Edit)
        {
            if (workerUIAdditive.activeSelf == true) workerUIAdditive.SetActive(false);
        }
    }
    public void UpdateTool()
    {
        if(gameManager.selectedCharacter.CompareTag("Player")
           &&inventory.currentTool == Inventory.Tool.Planting) SelectPlantTool();
        else if(gameManager.selectedCharacter.CompareTag("Player")
                &&inventory.currentTool == Inventory.Tool.Harvesting) SelectHarvestTool();
        else if(gameManager.selectedCharacter.CompareTag("Player")
                &&inventory.currentTool == Inventory.Tool.Watering) SelectWaterTool();
        else if(gameManager.selectedCharacter.CompareTag("Player")&&inventory.currentTool==Inventory.Tool.None) SelectNoneTool();
        else if(gameManager.selectedCharacter.CompareTag("Worker")
                &&gameManager.selectedCharacter.GetComponent<WorkerInteraction>().assignedWork == Inventory.Tool.Planting) SelectPlantTool();
        else if(gameManager.selectedCharacter.CompareTag("Worker")
                &&gameManager.selectedCharacter.GetComponent<WorkerInteraction>().assignedWork == Inventory.Tool.Harvesting) SelectHarvestTool();
        else if(gameManager.selectedCharacter.CompareTag("Worker")
                &&gameManager.selectedCharacter.GetComponent<WorkerInteraction>().assignedWork == Inventory.Tool.Watering) SelectWaterTool();
        else if(gameManager.selectedCharacter.CompareTag("Worker")
                &&gameManager.selectedCharacter.GetComponent<WorkerInteraction>().assignedWork == Inventory.Tool.None) SelectNoneTool();
    }
    public void UpdateCropImage()
    {
            if(gameManager.selectedCharacter.CompareTag("Player")&&
               cropImage.sprite != inventory.selectedCrop.icon) cropImage.sprite = _inventory.selectedCrop.icon;  
            else if(gameManager.selectedCharacter.CompareTag("Worker")&&
                    cropImage.sprite != 
                    gameManager.selectedCharacter.GetComponent<WorkerInteraction>().selectedcrop.icon)  
                cropImage.sprite = gameManager.selectedCharacter.GetComponent<WorkerInteraction>().selectedcrop.icon;
    }

    public void UpdateSeedAmount()
    {
        if (gameManager.selectedCharacter.CompareTag("Player")) 
            seedAmountDisplay.SetText(OptimizeSeedAmount(_inventory.GetSeedAmount(_inventory.selectedCrop)));
        else if (gameManager.selectedCharacter.CompareTag("Worker"))
            seedAmountDisplay.SetText(OptimizeSeedAmount(_inventory.
                GetSeedAmount(gameManager.selectedCharacter.GetComponent<WorkerInteraction>().selectedcrop)));
    }

    public string OptimizeSeedAmount(int seedAmount)
    {
        if (seedAmount >= 1000000f)
            return (seedAmount / 1000000f).ToString("0.#") + "M"; 
        if (seedAmount >= 1000f)
            return (seedAmount / 1000f).ToString("0.#") + "k"; 
        return seedAmount.ToString("0"); // less than 1000, show normally
    }

   public void SelectPlantTool()
    {
        if (gameManager.selectedCharacter.CompareTag("Player")) {
            _inventory.currentTool = Inventory.Tool.Planting;
            plantbutton.image.sprite = plantSelectedSprite;
            harvestbutton.image.sprite = harvestUnselectedSprite;
            waterbutton.image.sprite = waterUnselectedSprite; 
        }
        else if (gameManager.selectedCharacter.CompareTag("Worker")) {
            gameManager.selectedCharacter.GetComponent<WorkerInteraction>().assignedWork = Inventory.Tool.Planting;
            plantbutton.image.sprite = plantSelectedSprite;
            harvestbutton.image.sprite = harvestUnselectedSprite;
            waterbutton.image.sprite = waterUnselectedSprite;
        }
    }
    public void SelectHarvestTool()
    {
        if (gameManager.selectedCharacter.CompareTag("Player")) {
            _inventory.currentTool = Inventory.Tool.Harvesting;
            plantbutton.image.sprite = plantUnselectedSprite;
            harvestbutton.image.sprite = harvestSelectedSprite;
            waterbutton.image.sprite = waterUnselectedSprite;
        }
        else if (gameManager.selectedCharacter.CompareTag("Worker")) {
            gameManager.selectedCharacter.GetComponent<WorkerInteraction>().assignedWork = Inventory.Tool.Harvesting;
            plantbutton.image.sprite = plantUnselectedSprite;
            harvestbutton.image.sprite = harvestSelectedSprite;
            waterbutton.image.sprite = waterUnselectedSprite;
        }
    }
    public void SelectWaterTool()
    {
       if (gameManager.selectedCharacter.CompareTag("Player")) {
           _inventory.currentTool = Inventory.Tool.Watering;
            plantbutton.image.sprite = plantUnselectedSprite;
            harvestbutton.image.sprite = harvestUnselectedSprite;
            waterbutton.image.sprite = waterSelectedSprite;
        }
        else if (gameManager.selectedCharacter.CompareTag("Worker")) {
            gameManager.selectedCharacter.GetComponent<WorkerInteraction>().assignedWork = Inventory.Tool.Watering;
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
        if(gameManager.GetPreviousMode()==GameManager.GameMode.Edit) EditModeUI.SetActive(true);
        if(gameManager.GetPreviousMode()==GameManager.GameMode.Play) PlayModeUI.SetActive(true);
    }
    public void ExitGame()  
    {
        gameManager.ExitGame();
    }
}
