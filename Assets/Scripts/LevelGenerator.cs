using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject outsideCornerPrefab;
    public GameObject outsideWallPrefab;
    public GameObject insideCornerPrefab;
    public GameObject insideWallPrefab;
    public GameObject pelletPrefab;
    public GameObject powerPelletPrefab;
    public GameObject tJunctionPrefab;
    public List<GameObject> ghostPrefabs; 

    [Header("Level Maps")]
    [Tooltip("Define level maps for each scene by matching the scene name.")]
    public List<LevelData> levels = new List<LevelData>(); // Initialize the list

    private LevelData currentLevelData;

    [System.Serializable]
    public class LevelData
    {
        [Tooltip("Exact name of the scene (e.g., 'SampleScene').")]
        public string sceneName;

        [Tooltip("List of rows representing the level layout.")]
        public List<LevelRow> levelMap = new List<LevelRow>();

        [Tooltip("Whether the level map should be generated randomly.")]
        public bool isRandomlyGenerated = false;

        [Tooltip("Width of the map for random generation.")]
        public int mapWidth = 10;

        [Tooltip("Height of the map for random generation.")]
        public int mapHeight = 10;
    }

    [System.Serializable]
    public class LevelRow
    {
        [Tooltip("List of integers representing tile types for this row.")]
        public List<int> row = new List<int>();
    }

    private void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        // AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log("Current Scene: " + currentScene);
        currentLevelData = levels.Find(level => level.sceneName == currentScene);

        if (currentLevelData == null)
        {
            Debug.LogError("No LevelData defined for scene: " + currentScene);
            return;
        }

        Debug.Log("Found LevelData for scene: " + currentScene);

        if (currentLevelData.isRandomlyGenerated)
        {
            Debug.Log("Generating Level Map Randomly for scene: " + currentScene);
            GenerateRandomLevelMap(currentLevelData);
        }
        else
        {
            Debug.Log("Using predefined Level Map for scene: " + currentScene);
            if (currentLevelData.levelMap == null || currentLevelData.levelMap.Count == 0)
            {
                Debug.LogError("LevelMap is empty for scene: " + currentScene);
                return;
            }
        }

        List<LevelRow> levelMapToUse = currentLevelData.levelMap;
        int ghostIndex = 0; // Index to track the current ghost prefab for spawning

        for (int y = 0; y < levelMapToUse.Count; y++)
        {
            for (int x = 0; x < levelMapToUse[y].row.Count; x++)
            {
                Vector2 position = new Vector2(x, -y);
                int tileType = levelMapToUse[y].row[x];
                GameObject tile = null;
                Quaternion rotation = Quaternion.identity; // Default rotation

                switch (tileType)
                {
                    case 1: // Outside Corner
                        tile = outsideCornerPrefab;
                        rotation = DetermineOutsideCornerRotation(x, y);
                        break;

                    case 2: // Outside Wall
                        tile = outsideWallPrefab;
                        rotation = DetermineOutsideWallRotation(x, y);
                        break;

                    case 3: // Inside Corner
                        tile = insideCornerPrefab;
                        rotation = DetermineInsideCornerRotation(x, y);
                        break;

                    case 4: // Inside Wall
                        tile = insideWallPrefab;
                        rotation = DetermineInsideWallRotation(x, y);
                        break;

                    case 5: // Pellet
                        tile = pelletPrefab;
                        break;

                    case 6: // Power Pellet
                        tile = powerPelletPrefab;
                        break;

                    case 7: // T-junction
                        tile = tJunctionPrefab;
                        rotation = DetermineTJunctionRotation(x, y);
                        break;

                    case 8: // Ghost Spawn Point
                        if (ghostPrefabs != null && ghostPrefabs.Count > 0)
                        {
                            Instantiate(ghostPrefabs[ghostIndex % ghostPrefabs.Count], position, Quaternion.identity);
                            ghostIndex++;
                        }
                        break;
                }

                if (tile != null)
                {
                    Instantiate(tile, position, rotation, transform);
                    Debug.Log($"Instantiated tile type {tileType} at position ({x}, {y})");
                }
            }
        }
    }

    private void GenerateRandomLevelMap(LevelData levelData)
    {
        int width = levelData.mapWidth;
        int height = levelData.mapHeight;
        List<LevelRow> generatedMap = new List<LevelRow>();

        for (int y = 0; y < height; y++)
        {
            LevelRow row = new LevelRow();
            for (int x = 0; x < width; x++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    // Fixed borders: walls
                    if ((x == 0 || x == width - 1) && (y == 0 || y == height - 1))
                    {
                        // Corners
                        row.row.Add(1); // Outside Corner
                    }
                    else
                    {
                        // Walls
                        row.row.Add(2); // Outside Wall
                    }
                }
                else
                {
                    // Defines a wall probability, e.g., 30%
                    int tileType = Random.value < 0.3f ? 4 : 0; // Inside Wall or Empty
                    row.row.Add(tileType);
                }
            }
            generatedMap.Add(row);
        }

        // Assign Ghost Spawn Points
        int numberOfGhosts = Mathf.Min(ghostPrefabs.Count, 4); 
        List<Vector2> emptyPositions = new List<Vector2>();
        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                if (generatedMap[y].row[x] == 0)
                {
                    emptyPositions.Add(new Vector2(x, -y));
                }
            }
        }

        if (emptyPositions.Count >= numberOfGhosts)
        {
            for (int i = 0; i < numberOfGhosts; i++)
            {
                int index = Random.Range(0, emptyPositions.Count);
                Vector2 spawnPosition = emptyPositions[index];
                generatedMap[(int)-spawnPosition.y].row[(int)spawnPosition.x] = 8; // Ghost Spawn Point
                emptyPositions.RemoveAt(index);
            }
        }
        else
        {
            Debug.LogWarning("Not enough empty positions to place all ghosts.");
        }

        levelData.levelMap = generatedMap;
        Debug.Log("Random Level Map Generated with Ghost Spawn Points.");
    }

    // Placeholder methods for determining rotation based on tile type and position
    private Quaternion DetermineOutsideCornerRotation(int x, int y)
    {
        if (x == 0 && y == 0) return Quaternion.Euler(0, 0, 0); // Top-left
        if (x == currentLevelData.levelMap[0].row.Count - 1 && y == 0) return Quaternion.Euler(0, 0, -90); // Top-right
        if (x == 0 && y == currentLevelData.levelMap.Count - 1) return Quaternion.Euler(0, 0, 90); // Bottom-left
        if (x == currentLevelData.levelMap[0].row.Count - 1 && y == currentLevelData.levelMap.Count - 1) return Quaternion.Euler(0, 0, 180); // Bottom-right
        return Quaternion.identity;
    }

    private Quaternion DetermineOutsideWallRotation(int x, int y)
    {
        return (x == 0 || x == currentLevelData.levelMap[0].row.Count - 1) ? Quaternion.Euler(0, 0, 90) : Quaternion.identity;
    }

    private Quaternion DetermineInsideCornerRotation(int x, int y)
    {
        bool hasWallAbove = y > 0 && IsWall(currentLevelData.levelMap[y - 1].row[x]);
        bool hasWallRight = x < currentLevelData.levelMap[y].row.Count - 1 && IsWall(currentLevelData.levelMap[y].row[x + 1]);
        bool hasWallBelow = y < currentLevelData.levelMap.Count - 1 && IsWall(currentLevelData.levelMap[y + 1].row[x]);
        bool hasWallLeft = x > 0 && IsWall(currentLevelData.levelMap[y].row[x - 1]);

        if (hasWallAbove && hasWallLeft) return Quaternion.Euler(0, 0, 180);
        if (hasWallAbove && hasWallRight) return Quaternion.Euler(0, 0, 90);
        if (hasWallBelow && hasWallRight) return Quaternion.identity;
        if (hasWallBelow && hasWallLeft) return Quaternion.Euler(0, 0, 270);
        return Quaternion.identity;
    }

    private Quaternion DetermineInsideWallRotation(int x, int y)
    {
        bool hasWallAbove = y > 0 && IsWall(currentLevelData.levelMap[y - 1].row[x]);
        bool hasWallBelow = y < currentLevelData.levelMap.Count - 1 && IsWall(currentLevelData.levelMap[y + 1].row[x]);

        return (hasWallAbove && hasWallBelow) ? Quaternion.Euler(0, 0, 90) : Quaternion.identity;
    }

    private Quaternion DetermineTJunctionRotation(int x, int y)
    {
        return Quaternion.identity; 
    }

    private bool IsWall(int tileType)
    {
        return tileType == 1 || tileType == 2 || tileType == 3 || tileType == 4;
    }

    // Accessor method for CherryController to retrieve the level map
    public List<LevelRow> GetLevelMap()
    {
        return currentLevelData.levelMap;
    }
}
