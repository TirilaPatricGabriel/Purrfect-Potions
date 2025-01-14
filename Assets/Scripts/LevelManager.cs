using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        int nextLevelIndex = PlayerPrefs.GetInt("NextLevelIndex", 1);

        SceneManager.LoadScene(nextLevelIndex);
        SceneManager.LoadScene("SecondLevelScene");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
