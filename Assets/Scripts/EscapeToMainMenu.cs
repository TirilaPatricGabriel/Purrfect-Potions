using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeToMainMenu : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SaveAndLoadMainMenu();
        }
    }

    private void SaveAndLoadMainMenu()
    {
        if (DataPersistenceManager.Instance != null)
        {
            DataPersistenceManager.Instance.SaveGame();
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
