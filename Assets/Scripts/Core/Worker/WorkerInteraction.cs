using UnityEngine;
using System.Collections.Generic;

public class WorkerInteraction : MonoBehaviour
{
    Inventory inventory;
    public Inventory.Tool assignedWork=Inventory.Tool.None;
    public List<GameObject> selectedTiles = new List<GameObject>();
    public GameObject currentSelectedTile;
    public int seedinhands = 0;
    public bool workerSelected;
    public GameObject startTile;
    public GameObject endTile;
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
    }

    public void add(GameObject tile)
    {
        selectedTiles.Add(tile);
    }
    public void GetNearestTile()
    {
        float minDist = float.MaxValue;
        GameObject nearestTile = null;
        foreach (GameObject tile in selectedTiles)
        {
            float dist = (tile.transform.position - transform.position).magnitude;
            if (dist < minDist)
            {
                minDist = dist;
                nearestTile = tile;
            }
        }
        currentSelectedTile = nearestTile;
    }

    public void selectStartTile(GameObject Tile)
    {
        startTile = Tile;
    }
    public GameObject getStartTile()
    {
        return startTile;
    }
    public void selectEndTile(GameObject Tile)
    {
        endTile = Tile;
    }
    public GameObject getEndTile()
    {
        return endTile;
    }
    

    public void interact(GameObject tile)
    {
        if (assignedWork == Inventory.Tool.Planting)
        {
            if (inventory.HasSeeds(inventory.selectedCrop)&&tile.GetComponent<FarmTile>().currentState==FarmTile.TileState.Empty)
            {
                tile.GetComponent<FarmTile>().PlantCrop();
            }
        }
        else if(assignedWork==Inventory.Tool.Harvesting&&tile.GetComponent<FarmTile>().currentState==FarmTile.TileState.FullyGrown)
        {
            tile.GetComponent<FarmTile>().HarvestCrop();
        }
        else if(assignedWork==Inventory.Tool.Watering&&tile.GetComponent<FarmTile>().currentState==FarmTile.TileState.RequiresWater)
        {
            tile.GetComponent<FarmTile>().WaterCrop();
        }
    }

    public void clearTiles()
    {
        selectedTiles.Clear();
    }

    public void remove(GameObject tile)
    {
        selectedTiles.Remove(tile);
    }
    
}
