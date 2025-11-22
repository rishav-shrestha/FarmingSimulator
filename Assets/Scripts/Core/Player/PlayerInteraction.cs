
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    Inventory inventory;
    public List<GameObject> selectedTiles = new List<GameObject>();
    public GameObject currentSelectedTile;
    public int seedinhands = 0;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
    }
    public void addTile(GameObject tile)
    {

            switch(inventory.currentTool)
            {
                case Inventory.Tool.Planting:
                    if(inventory.HasSeeds(inventory.selectedCrop)&&tile.GetComponent<FarmTile>().currentState==FarmTile.TileState.Empty)
                    {
                    seedinhands++;
                    int seedinv = inventory.seedInventory[inventory.selectedCrop];
                    if (seedinv-seedinhands>=0)
                        add(tile);
                    }
                    break;
                case Inventory.Tool.Harvesting:
                    if(tile.GetComponent<FarmTile>().currentState == FarmTile.TileState.FullyGrown)
                    {
                        add(tile);
                    }
                    break;
                case Inventory.Tool.Watering:
                    if (tile.GetComponent<FarmTile>().currentState == FarmTile.TileState.RequiresWater)
                    {
                        add(tile);
                    }
                    break;
                default: 
                    break;
            }
       
    }

    private void add(GameObject tile)
    {
        tile.GetComponent<FarmTile>().action = inventory.currentTool;
        selectedTiles.Add(tile);
    }
    public void removeTile(GameObject tile) { 
        tile.GetComponent<FarmTile>().isSelected=false;
        if(tile.GetComponent<FarmTile>().action == Inventory.Tool.Planting)
        {
            seedinhands--;
        }
        tile.GetComponent<FarmTile>().action = Inventory.Tool.None;
        selectedTiles.Remove(tile);
        if (selectedTiles.Count > 0)
        {
            currentSelectedTile = selectedTiles[0];
        }
    }
    public void clearTiles() {
        for (int i = selectedTiles.Count - 1; i >= 0; i--)
        {
            selectedTiles[i].GetComponent<FarmTile>().isSelected=false;
        }
        selectedTiles.Clear();
    }
    public void interactTile(GameObject tile)
    {
        if(tile!=null)
        {
            switch (tile.GetComponent<FarmTile>().action)
            {
                case Inventory.Tool.Planting:
                    tile.GetComponent<FarmTile>().PlantCrop();
                    break;
                case Inventory.Tool.Harvesting:
                    tile.GetComponent<FarmTile>().HarvestCrop();
                    break;
                case Inventory.Tool.Watering:
                    tile.GetComponent<FarmTile>().WaterCrop();
                    break;
                default:
                    break;
            }
        } 
        tile.GetComponentInParent<FarmTile>().isCurrentSelected = false;
        removeTile(tile);
    }
    void Update()
    {
        if(selectedTiles.Count > 0)
        {
            currentSelectedTile = selectedTiles[0];
            currentSelectedTile.GetComponent<FarmTile>().isCurrentSelected=true;
        }
        else
        {
            currentSelectedTile = null;
        }
        for (int i = selectedTiles.Count - 1; i >= 0; i--)
        {
            selectedTiles[i].GetComponent<FarmTile>().isSelected=true;

        }
    }

}
