using System;
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
  public GameObject realStateHighlight;
  public GameObject hiringHighlight;
  public GameObject farmHighlight;
  
  private void Start()
  {
    if (SceneManager.GetActiveScene().name == "Game")
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
      UpdateLocationText();
      UpdateLocationHighlight();  
    }
  }
  public void LoadorExitWorldMap()
  {
    if (gameManager.GetGameMode() == GameManager.GameMode.Map)
    {
      gameManager.uIcontroller.mapUI.SetActive(false);
      gameManager.mapCam.enabled = false;
      switch (location)
      {
        case Location.Farm:
          gameManager.gameCam.enabled = true;
          gameManager.uIcontroller.farmUI.SetActive(true);
          gameManager.SetGameMode(GameManager.GameMode.Farm);
          break;
        case Location.Inventory:
          gameManager.gameCam.enabled = true;
          gameManager.uIcontroller.farmUI.SetActive(true);
          gameManager.SetGameMode(GameManager.GameMode.Storage);
          break;
        case Location.Shop:
          break;
        case Location.RealState:
          break;
        case Location.Hiring:
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
    else if (gameManager.GetGameMode() != GameManager.GameMode.Map)
    {
      gameManager.mapCam.enabled = true;
      gameManager.gameCam.enabled = false;
      gameManager.storageCam.enabled = false;
      gameManager.SetGameMode(GameManager.GameMode.Map);
      gameManager.uIcontroller.mapUI.SetActive(true);
      gameManager.uIcontroller.farmUI.SetActive(false);
    }
  }

  private void SetLocation(Location location)
  {
    this.location = location;
  }

  public void CycleLocation()
  {
    for (location++; location > Location.Hiring; location--)
    {
      SetLocation(location);
    }
    if(location==Location.Hiring) SetLocation(Location.Farm);
  }
  
  public enum Location
  {
    Farm,
    Inventory,
    Shop,
    RealState,
    Hiring
  }

  public void UpdateLocationText()
  {
    gameManager.uIcontroller.locationDisplay.SetText(location.ToString());
  }

  private void UpdateLocationHighlight()
  {
    if(inventoryHighlight.activeSelf==(location==Location.Inventory) && 
       shopHighlight.activeSelf==(location == Location.Shop) &&
       realStateHighlight.activeSelf== (location == Location.RealState)&&
       hiringHighlight.activeSelf ==(location == Location.Hiring) && 
       farmHighlight.activeSelf == (location == Location.Hiring))
      return;
    inventoryHighlight.SetActive(location==Location.Inventory);
    shopHighlight.SetActive(location == Location.Shop);
    realStateHighlight.SetActive(location == Location.RealState);
    hiringHighlight.SetActive (location == Location.Hiring);
    farmHighlight.SetActive(location == Location.Farm);
  }
}
