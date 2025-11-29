using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MapController : MonoBehaviour
{
  GameManager gameManager;
  public Location location;
  public Button inventoryButton;
  public Button shopButton;
  public Button realStateButton;
  public Button hiringButton;
  public Button farmButton;
  public GameObject inventoryHighlight;
  public GameObject shopHighlight;
  public GameObject realstateHighlight;
  public GameObject hiringHighlight;
  public GameObject farmHighlight;
  
  private void Start()
  {
    if (GameObject.FindGameObjectWithTag("GameManager") == null)
    {
      gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    
  }

  void Update()
  {
    if (GameObject.FindGameObjectWithTag("GameManager") != null)
    {
      gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    if (SceneManager.GetActiveScene().name == "Worldmap")
    {
      updateLocationButton();
      UpdateLocationHighlight();  
    }
  }
  public void LoadWorldMap()
  {
    gameManager.SetGameMode(GameManager.GameMode.Inactive);
    gameManager.gameObject.GetComponent<UIController>().playModeUI.SetActive(false);
    gameManager.HideGameLayer();
    SceneManager.LoadScene("WorldMap",LoadSceneMode.Additive);
  }
  public void ExitWorldMap()
  {
    if (location == Location.Farm)
    {
      SceneManager.UnloadSceneAsync("WorldMap");
      gameManager.showGameLayer();
      gameManager.gameObject.GetComponent<UIController>().playModeUI.SetActive(true);
      gameManager.SetGameMode(GameManager.GameMode.Play);
    }
    else if (location == Location.Inventory)
    {
      SceneManager.UnloadSceneAsync("WorldMap");
      SceneManager.LoadScene("Inventory",LoadSceneMode.Additive);
    }
    else if (location == Location.Shop)
    {
      SceneManager.UnloadSceneAsync("WorldMap");
      SceneManager.LoadScene("Shop",LoadSceneMode.Additive);
    }
    else if (location == Location.RealState)
    {
      SceneManager.UnloadSceneAsync("WorldMap");
      SceneManager.LoadScene("RealState",LoadSceneMode.Additive);
    }
    else if (location == Location.Hiring)
    {
      SceneManager.UnloadSceneAsync("WorldMap");
      SceneManager.LoadScene("Hiring",LoadSceneMode.Additive);
    }
  }

  public void setLocation(Location location)
  {
    this.location = location;
  }

  public void SetFarm()
  {
    setLocation(Location.Farm);
  }

  public void SetInventory()
  {
    setLocation(Location.Inventory);
  }
  public void SetShop()
  {
    setLocation(Location.Shop);
  }
  public void SetRealState()
  {
    setLocation(Location.RealState);
  }
  public void SetHiring()
  {
    setLocation(Location.Hiring);
  }
  
  public enum Location
  {
    Farm,
    Inventory,
    Shop,
    RealState,
    Hiring
  }

  public void updateLocationButton()
  {
    
    if (location == Location.Inventory)
    {
      inventoryButton.image.color = Color.yellow;
      shopButton.image.color = Color.blue;
      realStateButton.image.color = Color.blue;
      hiringButton.image.color = Color.blue;
      farmButton.image.color = Color.blue;
    }
    else if (location == Location.Shop)
    {
      inventoryButton.image.color = Color.blue;
      shopButton.image.color = Color.yellow;
      realStateButton.image.color = Color.blue;
      hiringButton.image.color = Color.blue;
      farmButton.image.color = Color.blue;
    }
    else if (location == Location.RealState)
    {
      inventoryButton.image.color = Color.blue;
      shopButton.image.color = Color.blue;
      realStateButton.image.color = Color.yellow;
      hiringButton.image.color = Color.blue;
      farmButton.image.color = Color.blue;
    }
    else if (location == Location.Hiring)
    {
      inventoryButton.image.color = Color.blue;
      shopButton.image.color = Color.blue;
      realStateButton.image.color = Color.blue;
      hiringButton.image.color = Color.yellow;
      farmButton.image.color = Color.blue;
    }
    else if (location == Location.Farm)
    {
      inventoryButton.image.color = Color.blue;
      shopButton.image.color = Color.blue;
      realStateButton.image.color = Color.blue;
      hiringButton.image.color = Color.blue;
      farmButton.image.color = Color.yellow;
    }
    else
    {
      inventoryButton.image.color = Color.blue;
      shopButton.image.color = Color.blue;
      realStateButton.image.color = Color.blue;
      hiringButton.image.color = Color.blue;
      farmButton.image.color = Color.blue;
    }
  }

  private void UpdateLocationHighlight()
  {
    if(inventoryHighlight.activeSelf==(location==Location.Inventory) && 
       shopHighlight.activeSelf==(location == Location.Shop) &&
       realstateHighlight.activeSelf== (location == Location.RealState)&&
       hiringHighlight.activeSelf ==(location == Location.Hiring) && 
       farmHighlight.activeSelf == (location == Location.Hiring))
      return;
    inventoryHighlight.SetActive(location==Location.Inventory);
    shopHighlight.SetActive(location == Location.Shop);
    realstateHighlight.SetActive(location == Location.RealState);
    hiringHighlight.SetActive (location == Location.Hiring);
    farmHighlight.SetActive(location == Location.Farm);
  }
}
