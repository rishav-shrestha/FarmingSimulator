using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FarmTile : MonoBehaviour
{

    public enum TileState
    {
        Empty,
        Growing,
        RequiresWater,
        FullyGrown,
        Dead
    }
    public TileState currentState = TileState.Empty;
    // Sprite for the farm tile when it's empty
    public Sprite emptyTileSprite;
    public Crop crop;
    public Inventory inventory;
    public int currentStage = 0;
    public float growthTimer = 0f;
    public float deathTimer = 0f;
    public bool isHovered = false;

    //outline variables
    public Color outlineColor = Color.yellow; // color of outline
    public float outlineScale = 1.05f;        // slightly bigger than main sprite
    public int outlineOrderOffset = 10;       // how much higher the outline should render
    public int aboveTileOffset = 20;          // how much higher the tile should render when hovered

    private GameObject outlineChild;
    public GameObject outlinePrehab;
    private SpriteRenderer mainRenderer;
    private SpriteRenderer tileRenderer;
    private int originalTileOrder;

    // Scaling variables
    public float hoverScale = 1.2f;    
    public float normalScale = 1f;     
    public float scaleSpeed = 5f;



    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();

        //outline 

        mainRenderer = GetComponent<SpriteRenderer>();
        tileRenderer = mainRenderer; // assuming tile itself is the main sprite
        originalTileOrder = tileRenderer.sortingOrder;

        CreateOutlineChild();
    }
    public void PlantCrop()
    {

        if (inventory.HasSeeds(inventory.selectedCrop) && inventory.currentTool == Inventory.Tool.Planting)
        {
            if (crop == null)
            {
                currentState = TileState.Growing;
                AudioManager.instance.playSFX(AudioManager.instance.plantSFX);
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
        if (inventory.currentTool == Inventory.Tool.Harvesting)
        {
            if (crop != null)
            {
                if (currentState == TileState.FullyGrown)
                {
                    Debug.Log("Crop harvested");
                    AudioManager.instance.playSFX(AudioManager.instance.harvestSFX);
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

    public void waterCrop()
    {
        if (currentState == TileState.RequiresWater && inventory.currentTool == Inventory.Tool.Watering)
        {
            AudioManager.instance.playEffects(AudioManager.instance.waterSFX);
            Debug.Log("Crop watered.");
            currentState = TileState.Growing;
            currentStage++;
        }
    }


    void Update()
    {
        if (crop != null)
        {
            if (currentStage < crop.totalStages - 1 && currentState == TileState.Growing)
            {
                growthTimer += Time.deltaTime;
                if (growthTimer >= crop.growthTime)
                {
                    growthTimer = 0f;
                    currentState = TileState.RequiresWater;
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
        this.GetComponent<SpriteRenderer>().sprite = crop != null ? crop.tile[crop.startingspriteIndex + currentStage - 1] : emptyTileSprite;
        // Smoothly scale the main tile
        float targetScale = isHovered ? hoverScale : normalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, Time.deltaTime * scaleSpeed);
        if (isHovered)
        {
            UpdateOutline(true);
        }
        else
        {
            UpdateOutline(false);
        }
    }

    public void UpdateOutline(bool outline)
    {
        if (outline)
        {
            Debug.Log("Hovering over tile - showing outline.");
            outlineChild.SetActive(true);
            tileRenderer.sortingOrder = originalTileOrder + aboveTileOffset; // bring tile above other tiles
        }
        else
        {
            outlineChild.SetActive(false);
            tileRenderer.sortingOrder = originalTileOrder; // restore original order
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
            sr.sortingOrder = tileRenderer.sortingOrder + outlineOrderOffset;

        // Hide by default
        outlineChild.SetActive(false);
    }
}