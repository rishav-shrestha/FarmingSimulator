using UnityEngine;
using System.Collections.Generic;

public class WorkerInteraction : MonoBehaviour
{
    Inventory _inventory;
    public Inventory.Tool assignedWork=Inventory.Tool.None;
    public List<GameObject> selectedTiles = new List<GameObject>();
    public GameObject currentSelectedTile;
    public GameObject startTile;
    public GameObject endTile;
    public WorkerData workerData;
    void Start()
    {
        workerData=this.GetComponent<WorkerData>();
        _inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
    }

    private void Update()
    {
        if (startTile != null && endTile != null)
        {
            SelectTilesBetween(startTile.GetComponent<FarmTile>(), endTile.GetComponent<FarmTile>());
            startTile = null;
            endTile = null;
        }
        
    }

    public void Add(GameObject tile)
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

    public void SetWork(Inventory.Tool tool)
    {
        assignedWork = tool;
    }

    public void SetCrop(Crop crop)
    {
        workerData.selectedcrop = crop;
    }

    public void SelectTilesBetween(FarmTile startTile, FarmTile endTile)
    {
        if (startTile == null || endTile == null) return;

        // Get the grid coordinates of startTile and endTile
        Vector2Int startPos = startTile.gridPos;  // You'll need to store this in FarmTile
        Vector2Int endPos = endTile.gridPos;

        int minX = Mathf.Min(startPos.x, endPos.x);
        int maxX = Mathf.Max(startPos.x, endPos.x);
        int minY = Mathf.Min(startPos.y, endPos.y);
        int maxY = Mathf.Max(startPos.y, endPos.y);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                FarmTile tile = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().tileManager.GetComponent<TileManager>().GetTileAt(x, y); 
                if (tile != null && !selectedTiles.Contains(tile.gameObject))
                {
                    tile.gameObject.GetComponent<SpriteRenderer>().color = Color.mediumPurple;
                    Add(tile.gameObject);
                }
            }
        }
    }

    public void Interact(GameObject tile)
    {
        if (assignedWork == Inventory.Tool.Planting)
        {
            if (_inventory.HasSeeds(_inventory.selectedCrop)&&tile.GetComponent<FarmTile>().currentState==FarmTile.TileState.Empty)
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

    public void ClearTiles()
    {
        selectedTiles.Clear();
    }

    public void Remove(GameObject tile)
    {
        selectedTiles.Remove(tile);
    }
    
}
