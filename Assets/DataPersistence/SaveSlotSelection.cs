using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotSelection : MonoBehaviour
{
    public Button[] saveSlotButtons; // Assign buttons in the Inspector
    public TextMeshProUGUI[] saveSlotLabels; // Optional: Assign TextMeshPro labels in the Inspector
    public bool isForFirstLevel = true;

    private void Start()
    {
        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            if (!isForFirstLevel)
            {
                int slotIndex = i + 5; // Capture the loop variable
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
                int slotIndex = i; // Capture the loop variable
                saveSlotButtons[i].onClick.AddListener(() => OnSaveSlotSelected(slotIndex));
                GameData loadedData = DataPersistenceManager.Instance?.GetLoadedData(slotIndex + 5);
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
    }

    public void OnSaveSlotSelected(int slotIndex)
    {
        // Set the save slot in DataPersistenceManager
        DataPersistenceManager.Instance.SetSaveSlot(slotIndex);

        // Start a new game immediately
        DataPersistenceManager.Instance.NewGame();
        Debug.Log($"Started a new game on Slot {slotIndex + 1}");
        if (!isForFirstLevel)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SecondLevelScene");
        } else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("FirstLevelScene");
        }
    }
}
