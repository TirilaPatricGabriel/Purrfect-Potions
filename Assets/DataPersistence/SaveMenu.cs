using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveMenu : MonoBehaviour
{
    public Button[] saveSlotButtons; // Assign buttons in the Inspector
    [SerializeField] private TextMeshProUGUI[] saveSlotLabels; // Assign TextMeshPro labels in the Inspector
    public Button newGameButton;
    public GameObject saveSlotSelectionScreen; // Assign the New Game Save Slot Selection Screen in the Inspector
    public bool isForFirstLevel = true;

    private void Start()
    {
        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            int slotIndex = isForFirstLevel ? i : i + 5;

            // Set up button click listener
            saveSlotButtons[i].onClick.AddListener(() => OnSaveSlotSelected(slotIndex));

            if (!isForFirstLevel)
            {
                saveSlotButtons[i].onClick.AddListener(() => OnSaveSlotSelected(slotIndex));
                GameData loadedData = DataPersistenceManager.Instance?.GetLoadedData(slotIndex);
                if (loadedData != null)
                {
                    saveSlotLabels[i].text = $"Save Slot {i + 1 + 5} - Gold: {loadedData.goldEarned}";
                }
                else
                {
                    saveSlotLabels[i].text = $"Save Slot {i + 1 + 5} - Empty";
                }
            }
            else
            {
                saveSlotButtons[i].onClick.AddListener(() => OnSaveSlotSelected(slotIndex));
                GameData loadedData = DataPersistenceManager.Instance?.GetLoadedData(slotIndex);
                if (loadedData != null)
                {
                    saveSlotLabels[i].text = $"Save Slot {i + 1} - Gold: {loadedData.goldEarned}";
                }
                else
                {
                    saveSlotLabels[i].text = $"Save Slot {i + 1} - Empty";
                }
            }
        }

        newGameButton.onClick.AddListener(OpenNewGameScreen);
    }

    private void OnSaveSlotSelected(int slotIndex)
    {
        DataPersistenceManager.Instance.SetSaveSlot(slotIndex);
        DataPersistenceManager.Instance.LoadGame();
        Debug.Log($"Loaded Save Slot {slotIndex + 1}");

        // Navigate to the appropriate scene
        string sceneName = isForFirstLevel ? "FirstLevelScene" : "SecondLevelScene";
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    private void OpenNewGameScreen()
    {
        saveSlotSelectionScreen.SetActive(true); // Show the Save Slot Selection Screen for New Game
        this.gameObject.SetActive(false); // Hide the Save Menu Screen
        Debug.Log("Navigated to Save Slot Selection for New Game");
    }
}
