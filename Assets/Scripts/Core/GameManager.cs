using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Inventory inventory;
    public GameObject TileManager;
    public GameObject player;
    public GameObject[] Workers;
    public GameObject selectedCharacter;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        selectedCharacter = player;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded: " + scene.name);
        if (scene.name == "Game")
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");  
            }
            else
            {
            GameObject oldPlayer = GameObject.FindGameObjectWithTag("Player");
            if (oldPlayer != null)
            {
                Destroy(oldPlayer);
            }
            Instantiate(player, player.transform.position, player.transform.rotation);
            }

            if (inventory == null)
            {
                inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
            }
            else
            {
            Inventory oldInventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
            if (oldInventory != null)
            {
                Destroy(oldInventory.gameObject);
            }
            Instantiate(inventory.gameObject, inventory.transform.position, inventory.transform.rotation);
            }

            if (TileManager == null)
            {
                TileManager = GameObject.FindGameObjectWithTag("TileManager");
            }
            else
            {
                GameObject oldTileManager = GameObject.FindGameObjectWithTag("TileManager");
                if (oldTileManager != null)
                {
                    Destroy(oldTileManager);
                }
                Instantiate(TileManager, TileManager.transform.position, TileManager.transform.rotation);
            }

            if (Workers == null)
            {
                Workers = GameObject.FindGameObjectsWithTag("Worker");
            }
            else
            {
                GameObject[] oldWorkers = GameObject.FindGameObjectsWithTag("Worker");
                foreach (GameObject oldworker in oldWorkers)
                {
                    Destroy( oldworker);
                }
                foreach (GameObject worker in Workers)
                {
                    Instantiate(worker, worker.transform.position, worker.transform.rotation);
                }
            }
        }
    }
    public void SelectCharacter(GameObject character)
    {
        selectedCharacter = character;
    }
    public void DeSelectCharacter()
    {
       selectedCharacter = null;
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
