using UnityEngine;

public class TileManager : MonoBehaviour
{
    public int width = 10;
    public int height = 10;

    public GameObject farmTilePrefab;

    public GameObject borderTopPrefab;
    public GameObject borderBottomPrefab;
    public GameObject borderLeftPrefab;
    public GameObject borderRightPrefab;
    public GameObject borderCornerPrefab;

    public GameObject grassTilePrefab;
    public int grassWidth = 1;

    public FarmTile[,] Tiles;

    private float _tileW;
    private float _tileH;

    void Start()
    {
        _tileW = farmTilePrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        _tileH = farmTilePrefab.GetComponent<SpriteRenderer>().bounds.size.y;

        GenerateFarm();
    }

    public void GenerateFarm()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        Tiles = new FarmTile[width, height];

        // ------------ FARM TILES ------------
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * _tileW, y * _tileH, 0);
                GameObject go = Instantiate(farmTilePrefab, pos, Quaternion.identity, transform);

                go.name = $"Tile_{x}_{y}";
                FarmTile ft = go.GetComponent<FarmTile>();

                if (ft != null)
                    ft.gridPos = new Vector2Int(x, y);

                Tiles[x, y] = ft;
            }
        }

        // ------------ BORDERS & CORNERS ------------
        for (int x = -1; x <= width; x++)
        {
            for (int y = -1; y <= height; y++)
            {
                bool isFarmTile = (x >= 0 && x < width && y >= 0 && y < height);
                if (isFarmTile) continue;

                Vector3 pos = new Vector3(x * _tileW, y * _tileH, 0);

                bool isCorner =
                    (x == -1 && y == -1) ||
                    (x == -1 && y == height) ||
                    (x == width && y == -1) ||
                    (x == width && y == height);

                if (isCorner)
                {
                    GameObject go = Instantiate(borderCornerPrefab, pos, Quaternion.identity, transform);

                    if (x == -1 && y == height) go.transform.rotation = Quaternion.Euler(0, 0, 0);
                    else if (x == width && y == height) go.transform.rotation = Quaternion.Euler(0, 0, -90);
                    else if (x == width && y == -1) go.transform.rotation = Quaternion.Euler(0, 0, 180);
                    else if (x == -1 && y == -1) go.transform.rotation = Quaternion.Euler(0, 0, 90);

                    go.name = $"Corner_{x}_{y}";
                    continue;
                }

                // Borders
                if (x == -1) Instantiate(borderLeftPrefab, pos, Quaternion.identity, transform);
                else if (x == width) Instantiate(borderRightPrefab, pos, Quaternion.identity, transform);
                else if (y == -1) Instantiate(borderBottomPrefab, pos, Quaternion.identity, transform);
                else if (y == height) Instantiate(borderTopPrefab, pos, Quaternion.identity, transform);
            }
        }

        // ------------ GRASS RING ------------
        for (int x = -1 - grassWidth; x <= width + grassWidth; x++)
        {
            for (int y = -1 - grassWidth; y <= height + grassWidth; y++)
            {
                bool isInsideFarm = (x >= 0 && x < width && y >= 0 && y < height);
                if (isInsideFarm) continue;

                bool isBorderArea = (x >= -1 && x <= width && y >= -1 && y <= height);
                if (isBorderArea) continue;

                Vector3 pos = new Vector3(x * _tileW, y * _tileH, 0);

                GameObject go = Instantiate(grassTilePrefab, pos, Quaternion.identity, transform);
                go.name = $"Grass_{x}_{y}";
            }
        }
    }


    public FarmTile GetTileAt(int x, int y)
    {
        if (Tiles == null) return null;
        if (x < 0 || x >= width || y < 0 || y >= height) return null;
        return Tiles[x, y];
    }
}
