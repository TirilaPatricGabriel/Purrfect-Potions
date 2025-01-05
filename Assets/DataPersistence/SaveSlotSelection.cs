using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotSelection : MonoBehaviour
{
    public Button[] saveSlotButtons; 
    public TextMeshProUGUI[] saveSlotLabels; 
    public bool isForFirstLevel = true;

    private void Start()
    {
        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            if (!isForFirstLevel)
            {
                int slotIndex = i + 5; 
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
                int slotIndex = i;
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
    }

    public void OnSaveSlotSelected(int slotIndex)
    {
        DataPersistenceManager.Instance.SetSaveSlot(slotIndex);

        // start new game
        DataPersistenceManager.Instance.NewGame();
        if (!isForFirstLevel)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SecondLevelScene");
        } else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("FirstLevelScene");
        }
    }
}
