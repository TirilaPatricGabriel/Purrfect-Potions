using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveMenu : MonoBehaviour
{
    public Button[] saveSlotButtons;
    [SerializeField] private TextMeshProUGUI[] saveSlotLabels; 
    public Button newGameButton;
    public GameObject saveSlotSelectionScreen; 
    public bool isForFirstLevel = true;

    private void Start()
    {
        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            int slotIndex = isForFirstLevel ? i : i + 5;

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
        
        string sceneName = isForFirstLevel ? "FirstLevelScene" : "SecondLevelScene";
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    private void OpenNewGameScreen()
    {
        saveSlotSelectionScreen.SetActive(true); 
        this.gameObject.SetActive(false); 
    }
}
