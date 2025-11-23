using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    [Header("Selected Items")]
    public Crop selectedCrop;
    public int seedamountdisplay;
    private CropDatabase _cropDatabase;
    [Header("Currencies")]
    [SerializeField] public int coins;


    [Header("Tool Settings")]
    public Tool currentTool = Tool.Planting;
    [Header("Inventory Data")]
    // how many of each crop (harvested items) you have
    public Dictionary<Crop, int> CropInventory = new Dictionary<Crop, int>();

    // how many seeds of each crop type you have
    public Dictionary<Crop, int> SeedInventory = new Dictionary<Crop, int>();


    void Start()
    {
        _cropDatabase = this.GetComponent<CropDatabase>();
        selectedCrop = _cropDatabase.crops[0]; // default to first crop
        // Initialize with zero or some starting amounts
        foreach (Crop crop in _cropDatabase.crops)
        {
            SeedInventory[crop] = 10;
            CropInventory[crop] = 0;
        }
    }
    void Update()
    {
        seedamountdisplay = SeedInventory[selectedCrop];
    }

    public void AddSeed(Crop crop, int amount)
    {
        if (!SeedInventory.ContainsKey(crop))
            SeedInventory[crop] = 0;
        SeedInventory[crop] += amount;
    }

    public bool HasSeeds(Crop crop, int amount = 1)
    {
        return SeedInventory.ContainsKey(crop) && SeedInventory[crop] >= amount;
    }
    public bool UseSeed(Crop crop, int amount = 1)
    {
        if (HasSeeds(crop,amount))
        {


            SeedInventory[crop] -= amount;
            return true;
        }
        if (SeedInventory[crop] -1 < 0)
        {
            Debug.LogWarning("Attempted to use more seeds than available!");
        }
        return false;
    }

    public void AddCrop(Crop crop, int amount)
    {
        if (!CropInventory.ContainsKey(crop))
            CropInventory[crop] = 0;
        CropInventory[crop] += amount;
    }
    public bool SellCrop(Crop crop, int amount = 1)
    {
        if (CropInventory.ContainsKey(crop) && SeedInventory[crop] >= amount)
        {
            SeedInventory[crop] -= amount;
            EarnCoins(amount);
            return true;
        }
        return false;
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            return true;
        }
        return false;
    }

    public void EarnCoins(int amount)
    {
        coins += amount;
    }
    public enum Tool
    {
        None,
        Planting,
        Harvesting,
        Watering
    }
    [System.Serializable]
    public class CropStack
    {
        public Crop crop;   
        public int amount; 
    }
}
