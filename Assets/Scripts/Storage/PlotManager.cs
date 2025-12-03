using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlotManager : MonoBehaviour
{
    public List<Plot> plots;
    public List<StorageUnit> storageUnits;
    public GameMode currentMode;
    public EditMode editMode;
    public StorageUnit selectedUnit;
    private void Start()
    {
        int id=0;
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Plot"))
        {
            plots.Add(p.GetComponent<Plot>());
            plots[id].id = id;
            id++;
            storageUnits.Add(p.GetComponent<Plot>().unit);
            
        }
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
