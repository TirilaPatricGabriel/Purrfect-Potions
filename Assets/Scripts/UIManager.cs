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

    public List<Texture> npcTextures;

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

        CheckAchievements();
    }

    public static void AddMoney(float amount)
    {
        totalMoney += amount;
    }

    // spawn new npc with random texture
    public void OrderCompletedOrCanceled()
    {
        if (npcPrefab != null && npcSpawnLocation != null)
        {
            GameObject npcInstance = Instantiate(npcPrefab, npcSpawnLocation.position, npcSpawnLocation.rotation);

            if (npcTextures != null && npcTextures.Count > 0)
            {
                Texture randomTexture = npcTextures[Random.Range(0, npcTextures.Count)];

                Transform headTransform = npcInstance.transform.Find("Head");
                if (headTransform != null)
                {
                    Renderer headRenderer = headTransform.GetComponent<Renderer>();
                    if (headRenderer != null)
                    {
                        Material newMaterial = new Material(Shader.Find("Standard")); 
                        newMaterial.mainTexture = randomTexture;

                        headRenderer.material = newMaterial;
                    }
                }
            }
        }
    }


    public void EndLevel()
    {
        if (DataPersistenceManager.Instance != null)
        {
            DataPersistenceManager.Instance.SaveGame();
        }
        PlayerPrefs.SetFloat("TotalMoney", totalMoney);
        PlayerPrefs.Save(); 
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
    }
}
