using UnityEngine;

[CreateAssetMenu(menuName = "Crops/Crop", fileName = "NewCrop")]
public class Crop : ScriptableObject
{
    public string cropName;
    public Sprite icon;
    public Sprite[] tile;
    public Sprite[] sprite;
    public Sprite[] storageunitsprite;
    public int growthTime = 120;
    public int deathTime = 240;
    public int minwatertime = 12;
    public int maxwatertime = 24;
    public int pileLifeTime = 600;
    public int storageUnitLifeTime = 3000;
    public int sellPrice = 10;
    public int seedBuyPrice = 10;
    public int maxStorageCapacity = 100;
    public int totalStages;
    public int harvestAmount;
    public bool locked;
}
