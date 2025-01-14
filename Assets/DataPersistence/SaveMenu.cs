using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; 

public class SaveMenu : MonoBehaviour
{
    public Button[] saveSlotButtons;                 
    [SerializeField] private TextMeshProUGUI[] saveSlotLabels; 
    [SerializeField] private GameObject[] tooltipPanels;       
    public Button newGameButton;                   
    public GameObject saveSlotSelectionScreen;       
    public bool isForFirstLevel = true;

    private void Start()
    {
        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            int slotIndex = isForFirstLevel ? i : i + 5;

            saveSlotButtons[i].onClick.AddListener(() => OnSaveSlotSelected(slotIndex));

            UpdateSaveSlotUI(i, slotIndex);

            AddTooltipEvents(saveSlotButtons[i], tooltipPanels[i]);
        }

        newGameButton.onClick.AddListener(OpenNewGameScreen);
    }

    private void UpdateSaveSlotUI(int buttonIndex, int slotIndex)
    {
        GameData loadedData = DataPersistenceManager.Instance?.GetLoadedData(slotIndex);

        if (loadedData != null)
        {
            saveSlotLabels[buttonIndex].text = $"Save Slot {slotIndex + 1} - Gold: {loadedData.goldEarned}";

            TextMeshProUGUI tooltipText = tooltipPanels[buttonIndex].GetComponentInChildren<TextMeshProUGUI>();
            if (tooltipText != null)
            {
                int minutes = (int)(loadedData.levelDuration / 60);
                int seconds = (int)(loadedData.levelDuration % 60);
                string formattedDuration = $"{minutes}m {seconds}s";
                string achievements = loadedData.unlockedAchievements.Count > 0
                    ? string.Join(", ", loadedData.unlockedAchievements)
                    : "No achievements unlocked";

                tooltipText.text =
                    $"Gold: {loadedData.goldEarned}\n" +
                    $"Duration: {formattedDuration}\n" +
                    $"{achievements}\n";
            }
        }
        else
        {
            saveSlotLabels[buttonIndex].text = $"Save Slot {slotIndex + 1} - Empty";

            TextMeshProUGUI tooltipText = tooltipPanels[buttonIndex].GetComponentInChildren<TextMeshProUGUI>();
            if (tooltipText != null)
            {
                tooltipText.text = "Empty Slot";
            }
        }

        tooltipPanels[buttonIndex].SetActive(false);
    }

    private void AddTooltipEvents(Button button, GameObject tooltipPanel)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        pointerEnter.callback.AddListener((_) => tooltipPanel.SetActive(true));
        trigger.triggers.Add(pointerEnter);

        EventTrigger.Entry pointerExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        pointerExit.callback.AddListener((_) => tooltipPanel.SetActive(false));
        trigger.triggers.Add(pointerExit);
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
