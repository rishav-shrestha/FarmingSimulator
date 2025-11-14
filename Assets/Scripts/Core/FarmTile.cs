using UnityEditor;
using UnityEngine;

public class FarmTile : MonoBehaviour
{
    // Sprite for the farm tile when it's empty
    public Sprite emptyTileSprite;
    public Crop crop;
    public Inventory inventory;
    public int currentStage = 0;
    public bool fullyGrown=false;
    public bool dead=false;
    public bool needsWater=false;
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
                if(fullyGrown)
                {
                    Debug.Log("Crop harvested");
                    AudioManager.instance.playSFX(AudioManager.instance.harvestSFX);
                    crop = null;
                    currentStage = 0;
                    growthTimer = 0f;
                    deathTimer = 0f;
                    fullyGrown = false;
                    dead = false;
                }
                else if(dead)
                {
                    Debug.Log("The crop is dead and cannot be harvested.");
                    crop = null;
                    currentStage = 0;
                    growthTimer = 0f;
                    deathTimer = 0f;
                    fullyGrown = false;
                    dead = false;
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
        if(needsWater && inventory.currentTool==Inventory.Tool.Watering)
        {
            AudioManager.instance.playEffects(AudioManager.instance.waterSFX);
            Debug.Log("Crop watered.");
            needsWater = false;
            currentStage++;
        }
    }


    void Update()
    {
        if(crop !=null)
        {
            if (currentStage < crop.totalStages - 1 && !needsWater)
            {
                growthTimer += Time.deltaTime;
                if (growthTimer >= crop.growthTime)
                {
                    growthTimer = 0f;
                    needsWater = true;
                }
            }
            if (currentStage == crop.totalStages - 1)
            {
                fullyGrown = true;
                deathTimer += Time.deltaTime;
                if (deathTimer >= crop.deathTime)
                {
                    fullyGrown = false;
                    currentStage++;
                    deathTimer = 0f;
                    dead = true;
                }
            }
        }
        else if(fullyGrown||dead||currentStage>0||growthTimer!=0||deathTimer!=0)
        {
            fullyGrown = false;
            dead = false;
            currentStage = 0;
            growthTimer = 0f;
            deathTimer = 0f;
        }


            this.GetComponent<SpriteRenderer>().sprite = crop != null ? crop.tile[crop.startingspriteIndex + currentStage - 1] : emptyTileSprite;
    }
}
