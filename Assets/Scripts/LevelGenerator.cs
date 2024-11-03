using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject outsideCornerPrefab;
    public GameObject outsideWallPrefab;
    public GameObject insideCornerPrefab;
    public GameObject insideWallPrefab;
    public GameObject pelletPrefab;
    public GameObject powerPelletPrefab;
    public GameObject tJunctionPrefab;

     private int[,] levelMap = {
        {1, 2, 2, 2, 2, 2, 2, 1},
        {2, 0, 5, 5, 5, 5, 5, 2},
        {2, 3, 5, 4, 4, 7, 5, 2},
        {2, 7, 4, 5, 4, 4, 4, 2},
        {2, 5, 4, 5, 5, 4, 5, 2},
        {2, 4, 5, 4, 4, 4, 7, 2},
        {2, 5, 5, 5, 5, 5, 6, 2},
        {1, 2, 2, 2, 2, 2, 2, 1}
    };


    private void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        for (int y = 0; y < levelMap.GetLength(0); y++)
        {
            for (int x = 0; x < levelMap.GetLength(1); x++)
            {
                Vector2 position = new Vector2(x, -y);
                int tileType = levelMap[y, x];
                GameObject tile = null;
                Quaternion rotation = Quaternion.identity; // Default rotation

                switch (tileType)
                {
                    case 1: // Outside Corner
                        tile = outsideCornerPrefab;
                        // Correct rotation for each corner based on position
                        if (x == 0 && y == 0) // Top-left corner
                            rotation = Quaternion.Euler(0, 0, 0);
                        else if (x == levelMap.GetLength(1) - 1 && y == 0) // Top-right corner
                            rotation = Quaternion.Euler(0, 0, -90);
                        else if (x == 0 && y == levelMap.GetLength(0) - 1) // Bottom-left corner
                            rotation = Quaternion.Euler(0, 0, 90);
                        else if (x == levelMap.GetLength(1) - 1 && y == levelMap.GetLength(0) - 1) // Bottom-right corner
                            rotation = Quaternion.Euler(0, 0, 180);
                        break;

                    case 2: // Outside Wall
                        tile = outsideWallPrefab;
                        // Rotate walls for vertical or horizontal orientation
                        rotation = (x == 0 || x == levelMap.GetLength(1) - 1) 
                            ? Quaternion.Euler(0, 0, 90) 
                            : Quaternion.identity;
                        break;

                    case 3: // Inside Corner
                        tile = insideCornerPrefab;
                        break;

                    case 4: // Inside Wall
                        tile = insideWallPrefab;
                        break;

                    case 5: // Pellet
                        tile = pelletPrefab;
                        break;

                    case 6: // Power Pellet
                        tile = powerPelletPrefab;
                        break;

                    case 7: // T-junction
                        tile = tJunctionPrefab;
                        break;
                }

                if (tile != null)
                {
                    Instantiate(tile, position, rotation, transform);
                }
            }
        }
    }
}
