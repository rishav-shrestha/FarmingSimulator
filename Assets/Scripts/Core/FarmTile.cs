using UnityEditor;
using UnityEngine;

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


    private void Start()
    {
        inventory= GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
    }
    public void PlantCrop()
    {
 
        if(inventory.HasSeeds(inventory.selectedCrop) && inventory.currentTool==Inventory.Tool.Planting)
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
        {   if (crop!=null)
            {
                if(currentState==TileState.FullyGrown)
                {
                    Debug.Log("Crop harvested");
                    AudioManager.instance.playSFX(AudioManager.instance.harvestSFX);
                    crop = null;
                    currentStage = 0;
                    growthTimer = 0f;
                    deathTimer = 0f;
                    currentState = TileState.Empty;
                }
                else if(currentState==TileState.Dead)
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
        if(currentState==TileState.RequiresWater && inventory.currentTool==Inventory.Tool.Watering)
        {
            AudioManager.instance.playEffects(AudioManager.instance.waterSFX);
            Debug.Log("Crop watered.");
            currentState = TileState.Growing;
            currentStage++;
        }
    }


    void Update()
    {
        if(crop !=null)
        {
            if (currentStage < crop.totalStages - 1 && currentState==TileState.Growing)
            {
                growthTimer += Time.deltaTime;
                if (growthTimer >= crop.growthTime)
                {
                    growthTimer = 0f;
                    currentState=TileState.RequiresWater;
                }
            }
            if (currentStage == crop.totalStages - 1)
            {
                currentState=TileState.FullyGrown;
                deathTimer += Time.deltaTime;
                if (deathTimer >= crop.deathTime)
                {
                    currentState=TileState.Dead;
                    currentStage++;
                    deathTimer = 0f;
                }
            }
        }
        else if(currentState!=TileState.Empty||currentStage>0||growthTimer!=0||deathTimer!=0)
        {
            currentState=TileState.Empty;
            currentStage = 0;
            growthTimer = 0f;
            deathTimer = 0f;
        }


            this.GetComponent<SpriteRenderer>().sprite = crop != null ? crop.tile[crop.startingspriteIndex + currentStage - 1] : emptyTileSprite;
    }
}
