using UnityEngine;

public class TagWalls : MonoBehaviour
{
    void Start()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Untagged");

        foreach (GameObject wall in walls)
        {
            if (wall.name.Contains("Wall"))
            {
                wall.tag = "Wall";
            }
        }

        Destroy(this);
    }
}
