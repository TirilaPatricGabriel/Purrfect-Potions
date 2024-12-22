using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
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
        int currentLevelIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        int nextLevelIndex = currentLevelIndex + 1;

        PlayerPrefs.SetFloat("TotalMoney", totalMoney);
        PlayerPrefs.SetInt("NextLevelIndex", nextLevelIndex);
        PlayerPrefs.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene("EndingLevelScene");
    }

    private void CheckAchievements()
    {
        foreach (var achievement in achievements)
        {
            PlayerPrefs.DeleteKey(achievement.achievementMessage); // WIP - Work In Progress

            if (totalMoney >= achievement.moneyThreshold && !PlayerPrefs.HasKey(achievement.achievementMessage))
            {
                achievementQueue.Enqueue(achievement.achievementMessage);

                PlayerPrefs.SetInt(achievement.achievementMessage, 1); 
                PlayerPrefs.Save();
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
        List<string> unlockedAchievements = new List<string>();

        foreach (var achievement in achievements)
        {
            if (PlayerPrefs.HasKey(achievement.achievementMessage))
            {
                unlockedAchievements.Add(achievement.achievementMessage);
            }
        }

        return unlockedAchievements;
    }

    public void ClearAchievements()
    {
        foreach (var achievement in achievements)
        {
            PlayerPrefs.DeleteKey(achievement.achievementMessage);
        }
        PlayerPrefs.Save();

        Debug.Log("All achievements cleared.");
    }
}
