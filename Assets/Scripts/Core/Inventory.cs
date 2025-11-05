using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    [Header("Selected Items")]
    public Crop selectedCrop;

    [Header("Currencies")]
    [SerializeField] public int coins = 0;


    [Header("Tool Settings")]
    public Tool currentTool = Tool.None;

    [Header("Inventory Data")]
    // how many of each crop (harvested items) you have
    public Dictionary<string, int> cropInventory = new Dictionary<string, int>();

    // how many seeds of each crop type you have
    public Dictionary<string, int> seedInventory = new Dictionary<string, int>();


    private void Start()
    {
        // Initialize with zero or some starting amounts
        cropInventory["Tomato"] = 0;
        seedInventory["Tomato"] = 5; // example: 5 tomato seeds at start
    }

    public void AddSeed(string cropName, int amount)
    {
        if (!seedInventory.ContainsKey(cropName))
            seedInventory[cropName] = 0;
        seedInventory[cropName] += amount;
    }

    public bool UseSeed(string cropName, int amount = 1)
    {
        if (seedInventory.ContainsKey(cropName) && seedInventory[cropName] >= amount)
        {
            seedInventory[cropName] -= amount;
            return true;
        }
        return false;
    }

    public void AddCrop(string cropName, int amount)
    {
        if (!cropInventory.ContainsKey(cropName))
            cropInventory[cropName] = 0;
        cropInventory[cropName] += amount;
    }
    public bool SellCrop(string cropName, int amount = 1)
    {
        if (cropInventory.ContainsKey(cropName) && seedInventory[cropName] >= amount)
        {
            seedInventory[cropName] -= amount;
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
