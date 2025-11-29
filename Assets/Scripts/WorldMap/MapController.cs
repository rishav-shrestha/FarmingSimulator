using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
  GameManager gameManager;
  private void Start()
  {
    gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
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
    SceneManager.UnloadSceneAsync("WorldMap");
    gameManager.showGameLayer();
    gameManager.gameObject.GetComponent<UIController>().playModeUI.SetActive(true);
    gameManager.SetGameMode(GameManager.GameMode.Play);
  }
}
