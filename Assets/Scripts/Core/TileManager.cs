using UnityEngine;

public class TileManager : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject tilePrefab;

    // Added: 2D array for easy access to instantiated FarmTile components
    public FarmTile[,] tiles;

    void Start()
    {
        GenerateFarm();
    }

    public void GenerateFarm()
    {
        // Clear existing children
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // Initialize the array with the current dimensions
        tiles = new FarmTile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x*tilePrefab.GetComponent<SpriteRenderer>().bounds.size.x, y* tilePrefab.GetComponent<SpriteRenderer>().bounds.size.y, 0);
                GameObject go = Instantiate(tilePrefab, pos, Quaternion.identity, transform);

                // Try to cache the FarmTile component for easy access
                FarmTile ft = go.GetComponent<FarmTile>();
                if (ft == null)
                {
                    Debug.LogWarning($"Instantiated tile at ({x},{y}) has no FarmTile component attached.");
                }
                tiles[x, y] = ft;
            }
        }
    }   

    // Optional helper: safe accessor
    public FarmTile GetTileAt(int x, int y)
    {
        if (tiles == null) return null;
        if (x < 0 || x >= width || y < 0 || y >= height) return null;
        return tiles[x, y];
    }
}
