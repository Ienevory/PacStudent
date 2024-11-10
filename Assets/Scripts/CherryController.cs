using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CherryController : MonoBehaviour
{
    public GameObject cherryPrefab;
    public float spawnInterval = 10f;
    public float cherryLifetime = 10f;

    [SerializeField] private LevelGenerator levelGenerator;

    private void Start()
    {
        if (levelGenerator == null)
        {
            levelGenerator = FindLevelGenerator();
        }

        StartCoroutine(SpawnCherry());
    }

    private LevelGenerator FindLevelGenerator()
    {
        return Object.FindFirstObjectByType<LevelGenerator>();
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

                AddComponentsToCherry(cherry);

                Destroy(cherry, cherryLifetime);
            }
        }
    }

    private void AddComponentsToCherry(GameObject cherry)
    {
        if (cherry.GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = cherry.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;

           
            collider.size = new Vector2(0.2f, 0.2f); 
            collider.offset = Vector2.zero;
        }

        cherry.tag = "Cherry";


    }

    private Vector2 GetRandomEmptyPosition()
    {
        List<Vector2> emptyPositions = new List<Vector2>();

        List<LevelGenerator.LevelRow> levelMap = levelGenerator.GetLevelMap();
        int height = levelMap.Count;
        int width = height > 0 ? levelMap[0].row.Count : 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < levelMap[y].row.Count; x++)
            {
                int tileType = levelMap[y].row[x];
                if (tileType == 0 || tileType == 5) 
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
            return Vector2.zero;
        }
    }
}
