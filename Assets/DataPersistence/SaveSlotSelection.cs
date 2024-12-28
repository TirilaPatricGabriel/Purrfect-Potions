using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotSelection : MonoBehaviour
{
    public Button[] saveSlotButtons; // Assign buttons in the Inspector
    public TextMeshProUGUI[] saveSlotLabels; // Optional: Assign TextMeshPro labels in the Inspector

    private void Start()
    {
        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            int slotIndex = i; // Capture the loop variable
            saveSlotButtons[i].onClick.AddListener(() => OnSaveSlotSelected(slotIndex));

            // Safely check if GetLoadedData works
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

    public void OnSaveSlotSelected(int slotIndex)
    {
        // Set the save slot in DataPersistenceManager
        DataPersistenceManager.Instance.SetSaveSlot(slotIndex);

        // Start a new game immediately
        DataPersistenceManager.Instance.NewGame();
        Debug.Log($"Started a new game on Slot {slotIndex + 1}");
        UnityEngine.SceneManagement.SceneManager.LoadScene("FirstLevelScene");
    }
}
