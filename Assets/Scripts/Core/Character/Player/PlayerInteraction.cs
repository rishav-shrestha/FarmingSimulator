
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    Inventory _inventory;
    public List<GameObject> selectedTiles = new List<GameObject>();
    public GameObject currentSelectedTile;
    public bool interacting;
    private PlayerData _playerData;
    public Slider interactionSlider;
    public float interactionTime;
    public float elapsed;
    void Start()
    {
        interactionSlider.gameObject.SetActive(false);
        _playerData=GetComponent<PlayerData>();
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
                    if(tile.GetComponent<FarmTile>().currentState == FarmTile.TileState.FullyGrown ||
                       tile.GetComponent<FarmTile>().currentState == FarmTile.TileState.Dead)
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
    public void CallInteract(GameObject Tile)
    {
        if(interacting) return;
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
            return maxTime - (maxTime - minTime) * Mathf.Log(_playerData.plantingskill + 1f) / Mathf.Log(9 + 1f);
            break;
        case Inventory.Tool.Watering:
            return maxTime - (maxTime - minTime) * Mathf.Log(_playerData.wateringskill + 1f) / Mathf.Log(9 + 1f);
            break;
        case Inventory.Tool.Harvesting:
            return maxTime - (maxTime - minTime) * Mathf.Log(_playerData.harvestingskill + 1f) / Mathf.Log(9 + 1f);
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
        interacting=false;
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
