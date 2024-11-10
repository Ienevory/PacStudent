using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CherryController : MonoBehaviour
{
    public GameObject cherryPrefab;
    public float spawnInterval = 10f;
    public float cherryLifetime = 10f;

    // Reference to LevelGenerator
    [SerializeField] private LevelGenerator levelGenerator;

    private void Start()
    {
        // If levelGenerator is not assigned in the Inspector, try finding it
        if (levelGenerator == null)
        {
            levelGenerator = FindLevelGenerator();
        }

        StartCoroutine(SpawnCherry());
    }

    private LevelGenerator FindLevelGenerator()
    {
        // Replace deprecated FindObjectOfType with FindFirstObjectByType
        return Object.FindFirstObjectByType<LevelGenerator>();
        // Alternatively, use FindAnyObjectByType for any instance:
        // return Object.FindAnyObjectByType<LevelGenerator>();
    }

    private IEnumerator SpawnCherry()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            Vector2 spawnPosition = GetRandomEmptyPosition();
            if (spawnPosition != Vector2.zero)
            {
                GameObject cherry = Instantiate(cherryPrefab, spawnPosition, Quaternion.identity);

                // Ensure the cherry has the correct components
                AddComponentsToCherry(cherry);

                // Destroy the cherry after a certain time if not collected
                Destroy(cherry, cherryLifetime);
            }
        }
    }

    private void AddComponentsToCherry(GameObject cherry)
    {
        // Add a BoxCollider2D if it's missing
        if (cherry.GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = cherry.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;

            // Reduce the collider size by 2/3
            // Assuming the original size is (1, 1), new size will be (0.333f, 0.333f)
            collider.size = new Vector2(0.2f, 0.2f); // Adjust as needed
            collider.offset = Vector2.zero; // Adjust if needed
        }

        // Set the tag to "Cherry"
        cherry.tag = "Cherry";

        // Set the layer if necessary
        // cherry.layer = LayerMask.NameToLayer("Default"); // Uncomment and adjust if needed
    }

    private Vector2 GetRandomEmptyPosition()
    {
        List<Vector2> emptyPositions = new List<Vector2>();

        List<LevelGenerator.LevelRow> levelMap = levelGenerator.GetLevelMap();
        int height = levelMap.Count;
        int width = height > 0 ? levelMap[0].row.Count : 0;

        // Collect all empty positions (tileType == 0 or 5)
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < levelMap[y].row.Count; x++)
            {
                int tileType = levelMap[y].row[x];
                if (tileType == 0 || tileType == 5) // Empty space or pellet
                {
                    Vector2 position = new Vector2(x, -y);
                    emptyPositions.Add(position);
                }
            }
        }

        if (emptyPositions.Count > 0)
        {
            // Select a random position from the list
            int index = Random.Range(0, emptyPositions.Count);
            return emptyPositions[index];
        }
        else
        {
            // No empty positions available
            return Vector2.zero;
        }
    }
}
