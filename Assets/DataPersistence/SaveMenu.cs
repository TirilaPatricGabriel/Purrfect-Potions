using UnityEngine;
using UnityEngine.UI;

public class SaveMenu : MonoBehaviour
{
    public Button[] saveSlotButtons; // Assign buttons in the Inspector
    public Button newGameButton;
    public GameObject saveSlotSelectionScreen; // Assign the New Game Save Slot Selection Screen in the Inspector

    private void Start()
    {
        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            int slotIndex = i; // Capture the loop variable
            saveSlotButtons[i].onClick.AddListener(() => OnSaveSlotSelected(slotIndex));
        }

        newGameButton.onClick.AddListener(OpenNewGameScreen);
    }

    private void OnSaveSlotSelected(int slotIndex)
    {
        DataPersistenceManager.Instance.SetSaveSlot(slotIndex);
        DataPersistenceManager.Instance.LoadGame();
        Debug.Log($"Loaded Save Slot {slotIndex + 1}");
        // Navigate to the game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("FirstLevelScene");
    }

    private void OpenNewGameScreen()
    {
        saveSlotSelectionScreen.SetActive(true); // Show the Save Slot Selection Screen for New Game
        this.gameObject.SetActive(false); // Hide the Save Menu Screen
        Debug.Log("Navigated to Save Slot Selection for New Game");
    }
}
