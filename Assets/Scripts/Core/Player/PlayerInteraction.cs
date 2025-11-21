
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public List<GameObject> selectedTiles = new List<GameObject>();
    public GameObject currentSelectedTile;
    public void addTile(GameObject tile)
    {
      selectedTiles.Add(tile);
    }
    public void removeTile(GameObject tile) { 
      selectedTiles.Remove(tile);
    }
    public void clearTiles() {
      selectedTiles.Clear();
    }
    public void interactTile(GameObject tile)
    {
        // Implement interaction logic here
        Debug.Log("Interacting with tile: " + tile.name);
        removeTile(tile);
    }
    void Update()
    {
        if(selectedTiles.Count > 0)
        {
            currentSelectedTile = selectedTiles[0];
        }
        else
        {
            currentSelectedTile = null;
        }

    }

}
