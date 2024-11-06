using UnityEngine;
using System.Collections;

public class CherryController : MonoBehaviour
{
    public GameObject cherryPrefab; 
    private float spawnInterval = 10f;

    private void Start()
    {
        StartCoroutine(SpawnCherry());
    }

    private IEnumerator SpawnCherry()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            Vector2 spawnPosition = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
            GameObject cherry = Instantiate(cherryPrefab, spawnPosition, Quaternion.identity);

            StartCoroutine(MoveCherry(cherry));
        }
    }

    private IEnumerator MoveCherry(GameObject cherry)
    {
        Vector2 targetPosition = new Vector2(-cherry.transform.position.x, -cherry.transform.position.y);
        float elapsedTime = 0;
        float duration = 5f;

        while (elapsedTime < duration)
        {
            cherry.transform.position = Vector3.Lerp(cherry.transform.position, targetPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(cherry);
    }
}
