using UnityEngine;
using UnityEngine.SceneManagement;

public class nextLevel : MonoBehaviour
{
    public void LoadNextLevel()
    {
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("" + currentSceneIndex+1);
        SceneManager.LoadScene(currentSceneIndex + 1);

        Debug.Log("Going to next level! " + currentSceneIndex + 1);
    }
}
