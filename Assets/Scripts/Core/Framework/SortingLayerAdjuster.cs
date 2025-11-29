using System;
using System.Collections.Generic;
using UnityEngine;

public class SortingLayerAdjuster : MonoBehaviour
{
    public List<GameObject> renderObjects;
    private GameManager _gameManager;
    [SerializeField]private int farmtilesortingorder;
    [SerializeField]private int outlineOffset;
    [SerializeField] private int hoveringOffset;

    void Start()
    {
        farmtilesortingorder=5;
        outlineOffset=5;
        hoveringOffset=10;
        _gameManager=GetComponent<GameManager>();
        renderObjects = new List<GameObject>();
        renderObjects.Add(GameObject.FindGameObjectWithTag("Player"));
        foreach (GameObject worker in GameObject.FindGameObjectsWithTag("Worker") ) renderObjects.Add(worker);
    }
    private void Update()
    {
        UpdateRenderObjects();
        SortRenderObjects();
    }

    public void SortRenderObjects()
    {
        List<(GameObject obj, float bottom)> sortable = new List<(GameObject, float)>();

        foreach (GameObject obj in renderObjects)
        {
            if (obj == null) continue;

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            float bottom = obj.transform.position.y - (sr.bounds.size.y * 0.5f);
            sortable.Add((obj, bottom));
        }
        
        sortable.Sort((b, a) => a.bottom.CompareTo(b.bottom)); 
       

        int orderStart = 30;
        int order = orderStart;
        
        for (int i = 0; i < sortable.Count; i++)
        {
            SpriteRenderer sr = sortable[i].obj.GetComponent<SpriteRenderer>();
            sr.sortingOrder = order;
            order++; 
        }
        foreach (GameObject farmTile in GameObject.FindGameObjectsWithTag("Farmtile"))
        {
            FarmTile tile = farmTile.GetComponent<FarmTile>();
            SpriteRenderer renderer = farmTile.GetComponent<SpriteRenderer>();
            renderer.sortingOrder = farmtilesortingorder;
            tile.outlineChild.GetComponent<SpriteRenderer>().sortingOrder = farmtilesortingorder - outlineOffset;
            if (tile.currentStage < 2)
            {
                tile.cropChild.GetComponent<SpriteRenderer>().sortingOrder = farmtilesortingorder + outlineOffset;
            }
            if (tile.outlined)
            {
                renderer.sortingOrder += hoveringOffset;
                if (tile.currentStage < 2)
                {
                    tile.cropChild.GetComponent<SpriteRenderer>().sortingOrder += hoveringOffset;
                }
                else
                {
                    tile.cropChild.GetComponent<SpriteRenderer>().sortingOrder += hoveringOffset*10; 
                }
                
                tile.outlineChild.GetComponent<SpriteRenderer>().sortingOrder += hoveringOffset;
            }
        }
    }
    void UpdateRenderObjects()
    {
        if (_gameManager.GetGameMode()==GameManager.GameMode.Pause) return;
        foreach (GameObject worker in GameObject.FindGameObjectsWithTag("Worker"))
        {
            if (!renderObjects.Contains(worker)) renderObjects.Add(worker);
        }
        foreach (GameObject farmTile in GameObject.FindGameObjectsWithTag("Farmtile"))
        {
          
            if (farmTile.GetComponent<FarmTile>().currentState != FarmTile.TileState.Empty)
            {
                if (!renderObjects.Contains(farmTile.GetComponent<FarmTile>().cropChild)) 
                    renderObjects.Add(farmTile.GetComponent<FarmTile>().cropChild);
                
            }
            else
            {
                if (renderObjects.Contains(farmTile.GetComponent<FarmTile>().cropChild)) 
                    renderObjects.Remove(farmTile.GetComponent<FarmTile>().cropChild);
            }
        }
    }
}
