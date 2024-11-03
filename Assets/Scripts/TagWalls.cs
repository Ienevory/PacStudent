using UnityEngine;

public class TagWalls : MonoBehaviour
{
    void Start()
    {
        // Find all GameObjects in the scene with "Wall" in their name
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Untagged");

        foreach (GameObject wall in walls)
        {
            if (wall.name.Contains("Wall"))
            {
                wall.tag = "Wall";
            }
        }

        // Remove this script from the GameObject after tagging
        Destroy(this);
    }
}
