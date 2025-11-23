using UnityEngine;

public class TileManager : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject farmTilePrefab;
    public GameObject borderPrehab;
    public GameObject outsidePrehab;

    // Added: 2D array for easy access to instantiated FarmTile components
    public FarmTile[,] Tiles;

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
        Tiles = new FarmTile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x*farmTilePrefab.GetComponent<SpriteRenderer>().bounds.size.x, y* farmTilePrefab.GetComponent<SpriteRenderer>().bounds.size.y, 0);
                GameObject go = Instantiate(farmTilePrefab, pos, Quaternion.identity, transform);

                // Try to cache the FarmTile component for easy access
                FarmTile ft = go.GetComponent<FarmTile>();
                
                go.name = $"Tile_{x}_{y}";
                if (ft == null)
                {
                    Debug.LogWarning($"Instantiated tile at ({x},{y}) has no FarmTile component attached.");
                }
                else
                {
                    ft.gridPos = new Vector2Int(x, y);  
                }
                Tiles[x, y] = ft;
            }
        }
    }   

    // Optional helper: safe accessor
    public FarmTile GetTileAt(int x, int y)
    {
        if (Tiles == null) return null;
        if (x < 0 || x >= width || y < 0 || y >= height) return null;
        return Tiles[x, y];
    }
}
