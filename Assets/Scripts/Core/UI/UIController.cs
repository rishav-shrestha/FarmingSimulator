using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public Inventory inventory;

    [Header("Buttons")]
    public Button plantbutton;
    public Button harvestbutton;
    public Button waterbutton;
    [Header("Sprites")]
    public Sprite plantSelectedSprite;
    public Sprite harvestSelectedSprite;
    public Sprite waterSelectedSprite;
    public Sprite plantUnselectedSprite;
    public Sprite harvestUnselectedSprite;
    public Sprite waterUnselectedSprite;
    public void SelectPlantTool()
    {
        inventory.currentTool = Inventory.Tool.Planting;
        plantbutton.image.sprite = plantSelectedSprite;
        harvestbutton.image.sprite = harvestUnselectedSprite;
        waterbutton.image.sprite = waterUnselectedSprite;
    }
    public void SelectHarvestTool()
    {
        inventory.currentTool = Inventory.Tool.Harvesting;
        plantbutton.image.sprite = plantUnselectedSprite;
        harvestbutton.image.sprite = harvestSelectedSprite;
        waterbutton.image.sprite = waterUnselectedSprite;
    }
    public void SelectWaterTool()
    {
        inventory.currentTool = Inventory.Tool.Watering;
        plantbutton.image.sprite = plantUnselectedSprite;
        harvestbutton.image.sprite = harvestUnselectedSprite;
        waterbutton.image.sprite = waterSelectedSprite;
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
