using UnityEngine;

[CreateAssetMenu(menuName = "Crops/Crop", fileName = "NewCrop")]
public class Crop : ScriptableObject
{
    public string cropName;
    public Sprite icon;
    public Sprite[] tile;
    public float growthTime = 24f;
    public float deathTime = 12f;
    public int sellPrice = 10;
    public int seedBuyPrice = 10;
    public int totalStages = 5;
    public bool locked;
}
