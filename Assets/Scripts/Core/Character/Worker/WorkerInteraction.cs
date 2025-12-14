using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class WorkerInteraction : MonoBehaviour
{
    Inventory _inventory;
    public Inventory.Tool assignedWork=Inventory.Tool.None;
    public List<GameObject> selectedTiles = new List<GameObject>();
    public List<GameObject> shownTiles = new List<GameObject>();
    public GameObject currentSelectedTile;
    public Slider interactionSlider;
    
    public GameObject startTile;
    public GameObject endTile;
    public Crop selectedcrop;
    public bool interacting;
    public float interactionTime;
    public float elapsed;
    
    private WorkerData _workerData;

    void Start()
    {
        interactionSlider.gameObject.SetActive(false);
        _workerData=GetComponent<WorkerData>();
        _inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        if (selectedcrop == null)
        {
            selectedcrop = _inventory.selectedCrop;
        }
    }

    private void Update()
    {
        if (startTile != null && endTile != null)
        {
            if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetEditMode() ==
                GameManager.EditMode.Add)
            {
                SelectTilesBetween(startTile.GetComponent<FarmTile>(), endTile.GetComponent<FarmTile>());
            }
            else if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetEditMode() ==
                     GameManager.EditMode.Remove)
            {
                RemoveTilesBetween(startTile.GetComponent<FarmTile>(), endTile.GetComponent<FarmTile>());
            }

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
    

    public void SetCrop(Crop crop)
    {
        selectedcrop = crop;
    }
    public void ShowTilesBetween(FarmTile startTile, FarmTile endTile)
    {
        shownTiles.Clear();
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
                if (tile != null)
                {
                    shownTiles.Add(tile.gameObject);
                }
            }
        }
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
                    Add(tile.gameObject);
                }
            }
        }
    }
    public void RemoveTilesBetween(FarmTile startTile, FarmTile endTile)
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
                if (tile != null && selectedTiles.Contains(tile.gameObject))
                {
                    Remove(tile.gameObject);
                }
            }
        }
    }

    public void CallInteract(GameObject Tile)
    {
        if(interacting) return;
        if (Tile == null)
        {
            Debug.Log("Tile is null");
            return;
        }
        StartCoroutine(Interact(Tile));
    }

    public float CalculateInteractionTime()
    {
        float maxTime = 1.5f;
        float minTime = 0.1f;
        Inventory.Tool action = currentSelectedTile.GetComponent<FarmTile>().action;
        switch (action)
        {
            case Inventory.Tool.Planting:
                return maxTime - (maxTime - minTime) * Mathf.Log(_workerData.plantingskill + 1f) / Mathf.Log(9 + 1f);
                break;
            case Inventory.Tool.Watering:
                return maxTime - (maxTime - minTime) * Mathf.Log(_workerData.wateringskill + 1f) / Mathf.Log(9 + 1f);
                break;
            case Inventory.Tool.Harvesting:
                return maxTime - (maxTime - minTime) * Mathf.Log(_workerData.harvestingskill + 1f) / Mathf.Log(9 + 1f);
                break;
        }
        
        return 0;
    }
    IEnumerator Interact(GameObject tile)
    {
        interactionTime = CalculateInteractionTime(); // your calculated duration
        elapsed = 0f;

        interacting = true;

        // Make sure the slider is visible and reset
        interactionSlider.gameObject.SetActive(true);
        interactionSlider.GetComponent<Slider>().value = 0f;

        // Loop for the duration
        while (elapsed < interactionTime)
        {
            elapsed += Time.deltaTime;                 // increase elapsed time
            interactionSlider.GetComponent<Slider>().value = elapsed / interactionTime; // update slider 0 → 1
            yield return null;                         // wait until next frame
        }

        // Ensure slider is full
        interactionSlider.GetComponent<Slider>().value = 1f;
        interactionSlider.gameObject.SetActive(false);

        // Call the tile interaction exactly after interactionTime
        InteractTile(tile);

        interacting = false;
    }
    public void InteractTile(GameObject tile)
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
        interacting=false;
        currentSelectedTile = null;
    }

    public void CycleCrop()
    {
        _inventory.CycleSelectedCrop(this);
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
