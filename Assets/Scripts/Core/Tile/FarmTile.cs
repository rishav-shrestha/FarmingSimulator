using UnityEngine;


public class FarmTile : MonoBehaviour
{

    public GameManager gameManager;
    public TileState currentState = TileState.Empty;
    // Sprite for the farm tile when it's empty
    public Sprite emptyTileSprite;
    public Crop crop;
    public Inventory inventory;
    [SerializeField] public int currentStage;
    [SerializeField] public float growthTimer ;
    [SerializeField] public float deathTimer ;
    [SerializeField] public float waterTimer;
    public bool isHovered;
    public bool isSelected;
    public bool isCurrentSelected;
    public Vector2Int gridPos;

    //outline variables
    public float outlineScale = 1.05f;        
    public int outlineOrderOffset = 10;       
    public int aboveTileOffset = 20;          
    

    //player interaction
    public Inventory.Tool action = Inventory.Tool.None;
    public Crop selectedCrop;

    public GameObject outlineChild;
    public GameObject outlinePrehab;
    private SpriteRenderer _mainRenderer;
    private SpriteRenderer _tileRenderer;
    private int _originalTileOrder;

    // Scaling variables
    public float hoverScale = 1.2f;    
    public float normalScale = 1f;     
    public float scaleSpeed = 5f;
    public bool outlined;

    //Crop Child
    public GameObject cropChild;
    private SpriteRenderer cropRenderer;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();

        //outline 

        _mainRenderer = GetComponent<SpriteRenderer>();
        _tileRenderer = _mainRenderer; // assuming tile itself is the main sprite
        _originalTileOrder = _tileRenderer.sortingOrder;

        //Crop Child
        cropRenderer = cropChild.GetComponent<SpriteRenderer>();

        CreateOutlineChild();
    }
    public void PlantCrop()
    {

        if (inventory.HasSeeds(selectedCrop) && action == Inventory.Tool.Planting)
        {
            if (crop == null)
            {
                currentState = TileState.Growing;
               AudioManager.Instance.PlayFarmSfx(AudioManager.Instance.plantSfx);
                crop = selectedCrop;
                inventory.UseSeed(selectedCrop);
            }
            selectedCrop = null;
        }
    }
    public void HarvestCrop()
    {
        if (action == Inventory.Tool.Harvesting)
        {
            if (crop != null)
            {
                if (currentState == TileState.FullyGrown)
                {
                   AudioManager.Instance.PlayFarmSfx(AudioManager.Instance.harvestSfx);
                   inventory.AddCrop(crop,1);
                    crop = null;
                    currentStage = 0;
                    growthTimer = 0f;
                    deathTimer = 0f;
                    currentState = TileState.Empty;
                }
                else if (currentState == TileState.Dead)
                {
                    crop = null;
                    currentStage = 0;
                    growthTimer = 0f;
                    deathTimer = 0f;
                    currentState = TileState.Empty;
                }
            }
        }
    }

    public void WaterCrop()
    {
        if (currentState == TileState.RequiresWater && action == Inventory.Tool.Watering)
        { 
            AudioManager.Instance.PlayFarmSfx(AudioManager.Instance.waterSfx);
            currentState = TileState.Growing;
            waterTimer--;
        }
    }
    
     public void PlantCrop(WorkerInteraction worker)
    {

        if (inventory.HasSeeds(worker.selectedcrop) && worker.assignedWork == Inventory.Tool.Planting)
        {
            if (crop == null)
            {
                currentState = TileState.Growing;
                AudioManager.Instance.PlayFarmSfx(AudioManager.Instance.plantSfx);
                crop = worker.selectedcrop;
                inventory.UseSeed(worker.selectedcrop);

            }
            else
            {
            }
        }
    }

    public void HarvestCrop(WorkerInteraction worker)
    {
        if (worker.assignedWork == Inventory.Tool.Harvesting)
        {
            if (crop != null)
            {
                if (currentState == TileState.FullyGrown)
                {
                    AudioManager.Instance.PlayFarmSfx(AudioManager.Instance.harvestSfx);
                    inventory.AddCrop(crop,1);
                    crop = null;
                    currentStage = 0;
                    growthTimer = 0f;
                    deathTimer = 0f;
                    currentState = TileState.Empty;
                }
                else if (currentState == TileState.Dead)
                {
                    crop = null;
                    currentStage = 0;
                    growthTimer = 0f;
                    deathTimer = 0f;
                    currentState = TileState.Empty;
                }
            }
        }
    }
    

    public void WaterCrop(WorkerInteraction worker)
    {
        if (currentState == TileState.RequiresWater && worker.assignedWork == Inventory.Tool.Watering)
        {
            AudioManager.Instance.PlayFarmSfx(AudioManager.Instance.waterSfx);
            currentState = TileState.Growing;
            waterTimer--;
        }
    }
    void Update()
    {
        if(gameManager.GetGameMode()==GameManager.GameMode.Pause) return;
        if (crop != null)
        {
            if (currentState != TileState.Empty && currentState != TileState.FullyGrown &&
                currentState != TileState.Dead)
            {
                if (waterTimer <= 0)
                {
                    waterTimer = Random.Range(crop.minwatertime, crop.maxwatertime);
                }

                if (waterTimer >= 1)
                {
                    waterTimer-=Time.deltaTime; 
                }
                else
                {
                    currentState = TileState.RequiresWater;
                }  
            }
            else
            {
                waterTimer = 0;
            }
                
            if (currentStage < crop.totalStages - 2 && currentState == TileState.Growing)
            {
                growthTimer += Time.deltaTime;
                if (growthTimer >= crop.growthTime)
                {
                    growthTimer = 0f;
                    currentStage++;
                }
            }
            if (currentStage == crop.totalStages - 2)
            {
                currentState = TileState.FullyGrown;
                deathTimer += Time.deltaTime;
                if (deathTimer >= crop.deathTime)
                {
                    currentState = TileState.Dead;
                    currentStage++;
                    deathTimer = 0f;
                }
            }
        }
        else if (currentState != TileState.Empty || currentStage > 0 || growthTimer != 0 || deathTimer != 0)
        {
            currentState = TileState.Empty;
            currentStage = 0;
            growthTimer = 0f;
            deathTimer = 0f;
        }
        switch (gameManager.graphicsMode)
        {
            case GameManager.GraphicsMode.Low:
                GetComponent<SpriteRenderer>().sprite = crop != null ? crop.tile[currentStage] : emptyTileSprite;
                break;
            case GameManager.GraphicsMode.High:
                if (gameManager.GetGameMode() == GameManager.GameMode.Edit)
                {
                    cropRenderer.sprite=null;
                    GetComponent<SpriteRenderer>().sprite = crop != null ? crop.tile[currentStage] : emptyTileSprite;
                    break;
                }
                if (crop != null)
                {
                    cropRenderer.sprite=crop.sprite[currentStage]; 
                }
                else
                {
                    cropRenderer.sprite=null;
                }
                GetComponent<SpriteRenderer>().sprite =emptyTileSprite;
                break;
        }
        // Smoothly scale the main tile
        float targetScale = isHovered ? hoverScale : normalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, Time.deltaTime * scaleSpeed);
        cropChild.transform.localScale = Vector3.Lerp(cropChild.transform.localScale, Vector3.one * targetScale, Time.deltaTime * scaleSpeed);
        if (gameManager.GetGameMode() == GameManager.GameMode.Play)
        {
            if(isCurrentSelected)
            {
                UpdateOutline(Color.yellow);
            }
            else if(isSelected)
            {
                if(action==Inventory.Tool.Planting)
                {
                    UpdateOutline(Color.green );
                }
                else if(action==Inventory.Tool.Watering)
                {
                    UpdateOutline(Color.blue);
                }
                else if(action==Inventory.Tool.Harvesting)
                {
                    UpdateOutline(Color.gray3);
                }
            }
            else if (isHovered)
            {
                UpdateOutline(Color.white);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.white;
                UpdateOutline();
            }   
        }
        
        else if(gameManager.GetGameMode() == GameManager.GameMode.Edit)
        {
            
            if (gameManager.GetEditMode() == GameManager.EditMode.Add ||
                gameManager.GetEditMode() == GameManager.EditMode.Remove)
            {
                if (this==gameManager.selectedCharacter.GetComponent<WorkerInteraction>().startTile)
                {
                    UpdateOutline(Color.darkBlue);
                }
                else if (this == gameManager.selectedCharacter.GetComponent<WorkerInteraction>().endTile)
                {
                    UpdateOutline(Color.red);
                }
                else if (gameManager.selectedCharacter.GetComponent<WorkerInteraction>().startTile == null && isHovered)
                {
                  UpdateOutline(Color.deepSkyBlue);
                }
                else if (gameManager.selectedCharacter.GetComponent<WorkerInteraction>().endTile == null && isHovered)
                {
                    UpdateOutline(Color.mediumVioletRed);
                    gameManager.selectedCharacter.GetComponent<WorkerInteraction>().ShowTilesBetween
                        (gameManager.selectedCharacter.GetComponent<WorkerInteraction>().startTile.GetComponent<FarmTile>(),this);
                }
                else
                {
                    UpdateOutline();
                }
            }

            if (gameManager.GetEditMode() == GameManager.EditMode.Normal)
            {
                if (isHovered)
                {
                    UpdateOutline(Color.white);
                }
                else
                {
                    UpdateOutline();
                } 
            }
            if (gameManager.selectedCharacter.GetComponent<WorkerInteraction>().startTile != null && gameManager.selectedCharacter.GetComponent<WorkerInteraction>().endTile == null && isHovered)
            {
                    gameManager.selectedCharacter.GetComponent<WorkerInteraction>().ShowTilesBetween
                        (gameManager.selectedCharacter.GetComponent<WorkerInteraction>().startTile.GetComponent<FarmTile>(),this);
                  
            }

            if (gameManager.GetEditMode() == GameManager.EditMode.Add)
            {
                if (gameManager.selectedCharacter.GetComponent<WorkerInteraction>().selectedTiles.Contains(this.gameObject))
                {
                    GetComponent<SpriteRenderer>().color = Color.green;
                }
                else if (gameManager.selectedCharacter.GetComponent<WorkerInteraction>().shownTiles.Contains(this.gameObject))
                {
                    GetComponent<SpriteRenderer>().color = Color.pink;
                } 
                else
                {
                    GetComponent<SpriteRenderer>().color = Color.white;
                }   
            }
            else
            {
                if (gameManager.selectedCharacter.GetComponent<WorkerInteraction>().shownTiles.Contains(this.gameObject)&&
                    gameManager.selectedCharacter.GetComponent<WorkerInteraction>().selectedTiles.Contains(this.gameObject))
                {
                    GetComponent<SpriteRenderer>().color = Color.pink;
                } else
                if (gameManager.selectedCharacter.GetComponent<WorkerInteraction>().selectedTiles.Contains(this.gameObject))
                {
                    GetComponent<SpriteRenderer>().color = Color.green;
                }
                else
                {
                    GetComponent<SpriteRenderer>().color = Color.white;
                }   
            }
        }
    }

    private void UpdateOutline(Color color,bool outline=true)
    {
        if (outline)
        {
            outlineChild.SetActive(true);
            outlineChild.GetComponent<SpriteRenderer>().color = color;
            if (gameManager.GetGameMode() == GameManager.GameMode.Play &&
                gameManager.graphicsMode == GameManager.GraphicsMode.High)
            {
                cropChild.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineThickness", 0.5f);
                cropChild.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", color);
            }
            outlined = true;
        }
        else
        {
            cropChild.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineThickness", 0);
            outlineChild.SetActive(false);
        }
    }

    private void UpdateOutline(bool outline=false)
    {
        if (outline)
        {
            outlineChild.SetActive(true);
            outlined = true;
        }
        else
        {
            cropChild.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineThickness", 0);
            outlineChild.SetActive(false);
            outlined = false;
        }
    }


    private void CreateOutlineChild()
    {
        if (outlineChild != null) return; 
        outlineChild = Instantiate(outlinePrehab, transform.position, Quaternion.identity, transform);
        outlineChild.transform.localPosition = Vector3.zero;
        outlineChild.transform.localRotation = Quaternion.identity;
        outlineChild.transform.localScale = Vector3.one * outlineScale;
        SpriteRenderer sr = outlineChild.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sortingOrder = _tileRenderer.sortingOrder + outlineOrderOffset;
        outlineChild.SetActive(false);
    }
    
    public enum TileState
    {
        Empty,
        Growing,
        RequiresWater,
        FullyGrown,
        Dead
    }
}