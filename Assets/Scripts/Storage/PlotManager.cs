using System;
using System.Collections.Generic;
using UnityEngine;

public class PlotManager : MonoBehaviour
{
    public List<GameObject> plots;
    public List<StorageUnit> storageUnits;
    public int unitCount;
    public int maxUnitCount;
    public StorageDisplay storageDisplay;
    public GameMode currentMode;
    public EditMode editMode;
    public StorageUnit selectedUnit;
    private void Start()
    {
        maxUnitCount = plots.Count;
        unitCount = 0;
        int id = 0;
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Plot"))
        {
         plots.Add(p);   
         storageUnits.Add(p.GetComponent<StorageUnit>());
         p.GetComponent<Plot>().plotID=id;
         p.name = $"Plot_{id}";
         id++;
        }
        storageDisplay = GetComponent<StorageDisplay>();
    }
    public enum EditMode
    {
        Relocate,
        Sell,
        Remove
    }
    public enum GameMode
    {
        Play,
        Edit,
        Pause,
        Inactive
    }
}
