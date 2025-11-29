
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    Inventory _inventory;
    public List<GameObject> selectedTiles = new List<GameObject>();
    public GameObject currentSelectedTile;

    void Start()
    {
        
        _inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
    }
    public void AddTile(GameObject tile)
    {

            switch(_inventory.currentTool)
            {
                case Inventory.Tool.Planting:
                    if (currentSelectedTile == null)
                  
                        if (_inventory.HasSeeds(_inventory.selectedCrop) &&
                            tile.GetComponent<FarmTile>().currentState == FarmTile.TileState.Empty)
                        {
                            Add(tile);
                            tile.GetComponent<FarmTile>().selectedCrop =
                                _inventory.selectedCrop;
                            break;
                        }
                   
                    if (_inventory.HasSeeds(_inventory.selectedCrop) &&
                        tile.GetComponent<FarmTile>().currentState == FarmTile.TileState.Empty)
                    {
                        int seedsPending = 0;
                        foreach (GameObject t in selectedTiles)
                        {
                            if (t.GetComponent<FarmTile>().selectedCrop == _inventory.selectedCrop)
                            {
                                seedsPending++;
                                Debug.Log(seedsPending);
                            }
                        }
                        if (_inventory.GetSeedAmount(_inventory.selectedCrop) - seedsPending > 0)
                        {
                            Add( tile);
                            tile.GetComponent<FarmTile>().selectedCrop =
                                _inventory.selectedCrop;
                        }
                    }

                    break;
                case Inventory.Tool.Harvesting:
                    if(tile.GetComponent<FarmTile>().currentState == FarmTile.TileState.FullyGrown)
                    {
                        Add(tile);
                    }
                    break;
                case Inventory.Tool.Watering:
                    if (tile.GetComponent<FarmTile>().currentState == FarmTile.TileState.RequiresWater)
                    {
                        Add(tile);
                    }
                    break;
            }
       
    }

    private void Add(GameObject tile)
    {
        tile.GetComponent<FarmTile>().action = _inventory.currentTool;
        selectedTiles.Add(tile);
    }
    public void RemoveTile(GameObject tile) { 
        tile.GetComponent<FarmTile>().isSelected=false;
        tile.GetComponent<FarmTile>().action = Inventory.Tool.None;
        selectedTiles.Remove(tile);
        if (selectedTiles.Count > 0)
        {
            currentSelectedTile = selectedTiles[0];
        }
    }
    public void ClearTiles() {
        for (int i = selectedTiles.Count - 1; i >= 0; i--)
        {
            selectedTiles[i].GetComponent<FarmTile>().isSelected=false;
        }
        selectedTiles.Clear();
    }
    public void InteractTile(GameObject tile)
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
            }
        } 
        tile.GetComponentInParent<FarmTile>().isCurrentSelected = false;
        RemoveTile(tile);
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
