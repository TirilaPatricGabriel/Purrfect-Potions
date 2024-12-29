using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void LoadNextLevel()
    {
        Debug.Log("LoadNextLevel called."); 
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        int nextLevelIndex = PlayerPrefs.GetInt("NextLevelIndex", 1);

        Debug.Log(currentSceneIndex);
        SceneManager.LoadScene(nextLevelIndex);
        SceneManager.LoadScene("SecondLevelScene");
    }
}
