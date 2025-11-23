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
    public FarmTile GetNearestTile(FarmTile.TileState state)
    {
        List<GameObject> conditionalTiles = new List<GameObject>();
        foreach (GameObject tile in selectedTiles)
        {
            if (tile.GetComponent<FarmTile>().currentState == state)
            {
                conditionalTiles.Add(tile);
            }
        }
        if (conditionalTiles.Count == 0) return null;
        if(conditionalTiles.Count==1) return conditionalTiles[0].GetComponent<FarmTile>();
        float minDist = float.MaxValue;
        foreach (GameObject tile in selectedTiles)
        {
            
        }
        GameObject nearestTile = null;
        foreach (GameObject tile in conditionalTiles)
        {
            float dist = (tile.transform.position - transform.position).magnitude;
            if (dist < minDist)
            {
                minDist = dist;
                nearestTile = tile;
            }
        }
        return nearestTile.GetComponent<FarmTile>();
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
        if(tile!=null)
        {
            switch (assignedWork)
            {
                case Inventory.Tool.Planting:
                    tile.GetComponent<FarmTile>().PlantCrop(this);
                    break;
                case Inventory.Tool.Harvesting:
                    tile.GetComponent<FarmTile>().HarvestCrop(this);
                    break;
                case Inventory.Tool.Watering:
                    tile.GetComponent<FarmTile>().WaterCrop(this);
                    break;
            }
        } 
        currentSelectedTile = null;
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
