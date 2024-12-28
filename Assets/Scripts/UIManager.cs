using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour, IDataPersistence
{
    public TextMeshProUGUI totalMoneyText;
    public static float totalMoney = 0f;

    public GameObject npcPrefab;
    public Transform npcSpawnLocation;

    public GameObject achievementPanel;
    public TextMeshProUGUI achievementText;
    private Queue<string> achievementQueue = new Queue<string>();
    private bool showingAchievement = false;

    private List<(float moneyThreshold, string achievementMessage)> achievements = new List<(float, string)>
    {
        (10, "Achievement Unlocked: Halfway There!"),
        (120, "Achievement Unlocked: Moneybags!"),
        (300, "Achievement Unlocked: Tycoon!")
    };

    private HashSet<string> unlockedAchievements = new HashSet<string>();

    public void LoadData(GameData data)
    {
        totalMoney = data.goldEarned;
        unlockedAchievements = new HashSet<string>(data.unlockedAchievements);
        UpdateTotalMoneyText();
    }

    public void SaveData(ref GameData data)
    {
        data.goldEarned = totalMoney;
        data.unlockedAchievements = new List<string>(unlockedAchievements);
    }

    public void UpdateTotalMoneyText()
    {
        if (totalMoneyText != null)
        {
            totalMoneyText.text = $"Total Money: ${totalMoney}";
        }
        else
        {
            Debug.LogWarning("Total money text not assigned in UIManager.");
        }

        CheckAchievements();
    }

    public static void AddMoney(float amount)
    {
        totalMoney += amount;
    }

    public void OrderCompletedOrCanceled()
    {
        if (npcPrefab != null && npcSpawnLocation != null)
        {
            Instantiate(npcPrefab, npcSpawnLocation.position, npcSpawnLocation.rotation);
            Debug.Log("New NPC spawned at the designated location.");
        }
        else
        {
            Debug.LogWarning("NPC prefab or spawn location not assigned in UIManager.");
        }
    }

    public void EndLevel()
    {
        // Save the data (handled externally by a Data Persistence Manager if needed)
        UnityEngine.SceneManagement.SceneManager.LoadScene("EndingLevelScene");
    }

    private void CheckAchievements()
    {
        foreach (var achievement in achievements)
        {
            if (totalMoney >= achievement.moneyThreshold && !unlockedAchievements.Contains(achievement.achievementMessage))
            {
                achievementQueue.Enqueue(achievement.achievementMessage);
                unlockedAchievements.Add(achievement.achievementMessage);
            }
        }

        if (!showingAchievement && achievementQueue.Count > 0)
        {
            StartCoroutine(ShowAchievement());
        }
    }

    private System.Collections.IEnumerator ShowAchievement()
    {
        showingAchievement = true;
        string message = achievementQueue.Dequeue();

        if (achievementPanel != null && achievementText != null)
        {
            achievementPanel.SetActive(true);
            achievementText.text = message;
            yield return new WaitForSeconds(3f);
            achievementPanel.SetActive(false);
        }

        showingAchievement = false;
    }

    public List<string> GetUnlockedAchievements()
    {
        return new List<string>(unlockedAchievements);
    }

    public void ClearAchievements()
    {
        unlockedAchievements.Clear();
        Debug.Log("All achievements cleared.");
    }
}
