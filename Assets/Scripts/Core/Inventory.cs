using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    [Header("Selected Items")]
    public Crop selectedCrop;
    public int seedamountdisplay = 0;
    private CropDatabase cropDatabase;
    [Header("Currencies")]
    [SerializeField] public int coins = 0;


    [Header("Tool Settings")]
    public Tool currentTool = Tool.Planting;
    [Header("Inventory Data")]
    // how many of each crop (harvested items) you have
    public Dictionary<Crop, int> cropInventory = new Dictionary<Crop, int>();

    // how many seeds of each crop type you have
    public Dictionary<Crop, int> seedInventory = new Dictionary<Crop, int>();


    void Start()
    {
        cropDatabase = this.GetComponent<CropDatabase>();
        selectedCrop = cropDatabase.crops[0]; // default to first crop
        // Initialize with zero or some starting amounts
        cropInventory[cropDatabase.crops[0]] = 0;
        seedInventory[cropDatabase.crops[0]] = 5; // example: 5 tomato seeds at start
    }
    void Update()
    {
        seedamountdisplay = seedInventory[selectedCrop];
    }

    public void AddSeed(Crop crop, int amount)
    {
        if (!seedInventory.ContainsKey(crop))
            seedInventory[crop] = 0;
        seedInventory[crop] += amount;
    }

    public bool HasSeeds(Crop crop, int amount = 1)
    {
        return seedInventory.ContainsKey(crop) && seedInventory[crop] >= amount;
    }
    public bool UseSeed(Crop crop, int amount = 1)
    {
        if (HasSeeds(crop,amount))
        {


            seedInventory[crop] -= amount;
            return true;
        }
        if (seedInventory[crop] -1 < 0)
        {
            Debug.LogWarning("Attempted to use more seeds than available!");
            return false;
        }
        return false;
    }

    public void AddCrop(Crop crop, int amount)
    {
        if (!cropInventory.ContainsKey(crop))
            cropInventory[crop] = 0;
        cropInventory[crop] += amount;
    }
    public bool SellCrop(Crop crop, int amount = 1)
    {
        if (cropInventory.ContainsKey(crop) && seedInventory[crop] >= amount)
        {
            seedInventory[crop] -= amount;
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
}
