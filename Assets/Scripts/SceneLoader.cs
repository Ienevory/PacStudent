using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Loads scene by it's name
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
