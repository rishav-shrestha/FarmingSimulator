using System;
using UnityEngine;

public class PlotManager : MonoBehaviour
{
    public GameObject[] plots;
    public StorageUnit[] storageUnits;
    public GameMode currentMode;
    public EditMode editMode;
    public StorageUnit selectedUnit;
    private void Start()
    {
        plots=GameObject.FindGameObjectsWithTag("Plot");
        foreach (GameObject p in plots)
        {
            p.GetComponent<Plot>().unit=storageUnits[UnityEngine.Random.Range(0,storageUnits.Length)];
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
