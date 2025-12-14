using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    [Header("Selected Items")]
    public Crop selectedCrop;
    public int seedamountdisplay;
    public CropDatabase _cropDatabase;
    [Header("Currencies")]
    public int coins=500;


    [Header("Tool Settings")]
    public Tool currentTool = Tool.Planting;
    [Header("Inventory Data")]
    // List for seeds
    public List<CropStack> seedInventory = new List<CropStack>();

    // List for harvested crops
    public List<CropStack> cropInventory = new List<CropStack>();

    
    void Start()
    {
        _cropDatabase = GetComponent<CropDatabase>();
        selectedCrop = _cropDatabase.crops[0];
        // Initialize seeds/crops for all crops in database
        foreach (Crop crop in _cropDatabase.crops)
        {
            seedInventory.Add(new CropStack { crop = crop, amount = 10 }); // starting seeds
            cropInventory.Add(new CropStack { crop = crop, amount = 0 });  // no harvested crops yet
        }
    }
    void Update()
    {
        seedamountdisplay=GetSeedAmount(selectedCrop);
    }

    public void SetSelectedCrop(Crop crop)
    {
        selectedCrop = crop;
    }

    public void CycleSelectedCrop()
    {
        if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().selectedCharacter
            .CompareTag("Player"))
        {
            int index = _cropDatabase.crops.IndexOf(selectedCrop);
            index = (index + 1) % _cropDatabase.crops.Count;
            SetSelectedCrop( _cropDatabase.crops[index]);
        }
    }
    public void CycleSelectedCrop(WorkerInteraction worker)
    {
        if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().selectedCharacter
            .CompareTag("Worker"))
        {
            int index = _cropDatabase.crops.IndexOf(worker.selectedcrop);
            index = (index + 1) % _cropDatabase.crops.Count;
            worker.SetCrop( _cropDatabase.crops[index]);  
        }
    }
    
    // --- Seed methods ---
    public void AddSeed(Crop crop, int amount)
    {
        CropStack stack = seedInventory.Find(x => x.crop == crop);
        if (stack != null)
            stack.amount += amount;
        else
            seedInventory.Add(new CropStack { crop = crop, amount = amount });
    }

    public int GetSeedAmount(Crop crop)
    {
        CropStack stack = seedInventory.Find(x => x.crop == crop);
        return stack.amount;
    }
    public bool HasSeeds(Crop crop)
    {
        int amount =GetSeedAmount(crop);
        if (amount == 0)
        {
            return false;
        }
      return true;
    }

    public bool UseSeed(Crop crop, int amount = 1)
    {
        CropStack stack = seedInventory.Find(x => x.crop == crop);
        if (stack != null && stack.amount >= amount)
        {
            stack.amount -= amount;
            return true;
        }
        
        return false;
    }

    // --- Crop methods ---
    public void AddCrop(Crop crop, int amount)
    {
        CropStack stack = cropInventory.Find(x => x.crop == crop);
        if (stack != null)
            stack.amount += amount;
        else
            cropInventory.Add(new CropStack { crop = crop, amount = amount });
    }
    public bool RemoveCrop(Crop crop, int amount = 1)
    {
        CropStack stack = cropInventory.Find(x => x.crop == crop);
        if (stack != null && stack.amount >= amount)
        {
            stack.amount -= amount;
            return true;
        }
        return false;
    }

    public int GetCropAmount(Crop crop)
    {
        CropStack stack = cropInventory.Find(x => x.crop == crop);
        return stack.amount;
    }
    public bool SellCrop(Crop crop, int amount = 1)
    {
        CropStack stack = cropInventory.Find(x => x.crop == crop);
        if (stack != null && stack.amount >= amount)
        {
            stack.amount -= amount;
            EarnCoins(amount); // your existing method
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

    public void BuySelectedCropSeed()
    {
        if (gameObject.GetComponent<GameManager>().selectedCharacter.CompareTag("Player"))
        {
            if (SpendCoins(selectedCrop.seedBuyPrice*5))
            {
                AddSeed( selectedCrop, 5);
            } 
        }
        else if (gameObject.GetComponent<GameManager>().selectedCharacter.CompareTag("Worker"))
        {
            if (SpendCoins(gameObject.GetComponent<GameManager>().selectedCharacter.
                    GetComponent<WorkerInteraction>().selectedcrop.seedBuyPrice*5))
            {
                AddSeed( gameObject.GetComponent<GameManager>().selectedCharacter.
                    GetComponent<WorkerInteraction>().selectedcrop, 5);
            } 
        }
    }
    public void SellAllCrops()
    {
        foreach (CropStack stack in cropInventory)
        {
            coins += stack.amount * stack.crop.sellPrice;
            stack.amount = 0;
        }
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
