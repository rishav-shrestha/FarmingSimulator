using System;
using UnityEngine;

public class Plot : MonoBehaviour
{
    public StorageUnit unit;
    public GameObject highlight;
    public bool hovered;
    public bool hasUnit;
    public int id;

    private void Update()
    {
        if (hasUnit) unit.gameObject.SetActive(true);
        else unit.gameObject.SetActive(false);
        
        if(hovered) highlight.SetActive(true);
        else highlight.SetActive(false);
    }
    public void AddNewUnit()
    {
        unit.crop=null;
        unit.cropStored=0;
        unit.gameObject.SetActive(true);
        hasUnit = true;
    }
    public void RemoveUnit()
    {
        unit.crop=null;
        unit.cropStored=0;
        unit.gameObject.SetActive(false);
        hasUnit = false;
    }

    public void SetUnit(StorageUnit unit)
    {
        this.unit = unit;
        hasUnit = true;
        unit.gameObject.SetActive(true);
    }
}
