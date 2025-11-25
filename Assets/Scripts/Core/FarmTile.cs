using UnityEngine;


public class FarmTile : MonoBehaviour
{

    
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
    public Color outlineColor = Color.yellow; 
    public float outlineScale = 1.05f;        
    public int outlineOrderOffset = 10;       
    public int aboveTileOffset = 20;          
    

    //player interaction
    public Inventory.Tool action = Inventory.Tool.None;

    public GameObject outlineChild;
    public GameObject outlinePrehab;
    private SpriteRenderer _mainRenderer;
    private SpriteRenderer _tileRenderer;
    private int _originalTileOrder;

    // Scaling variables
    public float hoverScale = 1.2f;    
    public float normalScale = 1f;     
    public float scaleSpeed = 5f;



    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();

        //outline 

        _mainRenderer = GetComponent<SpriteRenderer>();
        _tileRenderer = _mainRenderer; // assuming tile itself is the main sprite
        _originalTileOrder = _tileRenderer.sortingOrder;

        CreateOutlineChild();
    }
    public void PlantCrop()
    {

        if (inventory.HasSeeds(inventory.selectedCrop) && action == Inventory.Tool.Planting)
        {
            if (crop == null)
            {
                currentState = TileState.Growing;
               AudioManager.Instance.PlayEffects(AudioManager.Instance.plantSfx);
                crop = inventory.selectedCrop;
                inventory.UseSeed(inventory.selectedCrop);
                Debug.Log("Tile is now planted.");

            }
            else
            {
                Debug.Log("Tile is already planted.");
            }
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
                   AudioManager.Instance.PlayEffects(AudioManager.Instance.harvestSfx);
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
            AudioManager.Instance.PlayEffects(AudioManager.Instance.waterSfx);
            currentState = TileState.Growing;
            waterTimer--;
        }
    }
    
     public void PlantCrop(WorkerInteraction worker)
    {

        if (inventory.HasSeeds(inventory.selectedCrop) && worker.assignedWork == Inventory.Tool.Planting)
        {
            if (crop == null)
            {
                currentState = TileState.Growing;
                AudioManager.Instance.PlayEffects(AudioManager.Instance.plantSfx);
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
                    Debug.Log("Crop harvested");
                    AudioManager.Instance.PlayEffects(AudioManager.Instance.harvestSfx);
                    crop = null;
                    currentStage = 0;
                    growthTimer = 0f;
                    deathTimer = 0f;
                    currentState = TileState.Empty;
                }
                else if (currentState == TileState.Dead)
                {
                    Debug.Log("The crop is dead and cannot be harvested.");
                    crop = null;
                    currentStage = 0;
                    growthTimer = 0f;
                    deathTimer = 0f;
                    currentState = TileState.Empty;
                }
                else
                {
                    Debug.Log("The crop is not fully grown yet.");
                }
            }
            else
            {
                Debug.Log("No crop to harvest on this tile.");
            }
        }
    }

    public void WaterCrop(WorkerInteraction worker)
    {
        if (currentState == TileState.RequiresWater && worker.assignedWork == Inventory.Tool.Watering)
        {
            AudioManager.Instance.PlayEffects(AudioManager.Instance.waterSfx);
            currentState = TileState.Growing;
        }
    }


    void Update()
    {
        if (crop != null)
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
                
            if (currentStage < crop.totalStages - 1 && currentState == TileState.Growing)
            {
                growthTimer += Time.deltaTime;
                if (growthTimer >= crop.growthTime)
                {
                    growthTimer = 0f;
                    currentStage++;
                }
            }
            if (currentStage == crop.totalStages - 1)
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
        this.GetComponent<SpriteRenderer>().sprite = crop != null ? crop.tile[currentStage] : emptyTileSprite;
        // Smoothly scale the main tile
        float targetScale = isHovered ? hoverScale : normalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, Time.deltaTime * scaleSpeed);
        
        if(isCurrentSelected)
        {
            UpdateOutline(true, Color.yellow,2);
        }
        else if(isSelected)
        {
            if(action==Inventory.Tool.Planting)
            {
                UpdateOutline(true, Color.green, 1);
            }
            else if(action==Inventory.Tool.Watering)
            {
                UpdateOutline(true, Color.blue, 1);
            }
            else if(action==Inventory.Tool.Harvesting)
            {
                UpdateOutline(true, Color.gray3, 1);
            }
        }
        else if (isHovered)
        {
            UpdateOutline(true,Color.white,1);
        }
        else
        {
            UpdateOutline(false);
        }
    }

    public void UpdateOutline(bool outline, Color color,int order)
    {
        if (outline)
        {
            outlineChild.SetActive(true);
            outlineChild.GetComponent<SpriteRenderer>().color = color;
            outlineChild.GetComponent<SpriteRenderer>().sortingOrder = order+_originalTileOrder;
            _tileRenderer.sortingOrder = _originalTileOrder + aboveTileOffset+order; // bring tile above other tiles
        }
        else
        {
            outlineChild.GetComponent<SpriteRenderer>().sortingOrder = _originalTileOrder;
            outlineChild.SetActive(false);
            _tileRenderer.sortingOrder = _originalTileOrder; // restore original order
        }
    }


    public void UpdateOutline(bool outline)
    {
        if (outline)
        {
            outlineChild.SetActive(true);
        }
        else
        {
            outlineChild.GetComponent<SpriteRenderer>().sortingOrder = _originalTileOrder;
            outlineChild.SetActive(false);
            _tileRenderer.sortingOrder = _originalTileOrder; // restore original order
        }
    }


    void CreateOutlineChild()
    {
        if (outlineChild != null) return; // safety check

        // Instantiate the prefab at the same position and parent it
        outlineChild = Instantiate(outlinePrehab, transform.position, Quaternion.identity, transform);

        // Reset local position/rotation just in case
        outlineChild.transform.localPosition = Vector3.zero;
        outlineChild.transform.localRotation = Quaternion.identity;

        // Optional: scale slightly bigger than the tile
        outlineChild.transform.localScale = Vector3.one * outlineScale;

        // Set the Order in Layer relative to the main tile
        SpriteRenderer sr = outlineChild.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sortingOrder = _tileRenderer.sortingOrder + outlineOrderOffset;

        // Hide by default
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