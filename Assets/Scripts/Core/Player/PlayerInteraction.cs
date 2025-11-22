
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Inventory inventory;
    public List<GameObject> selectedTiles = new List<GameObject>();
    public GameObject currentSelectedTile;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
    }
    public void addTile(GameObject tile)
    {
      selectedTiles.Add(tile);
    }
    public void removeTile(GameObject tile) { 
        tile.GetComponent<FarmTile>().isSelected=false;
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
            Debug.Log("Interacting with tile: " + tile.name + "located at" + tile.transform.position);
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
