using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI totalMoneyText;
    public static float totalMoney = 0f;

    public GameObject npcPrefab;  
    public Transform npcSpawnLocation; 

    public void UpdateTotalMoneyText()
    {
        if (totalMoneyText != null)
        {
            totalMoneyText.text = $"Total Money: ${totalMoney}";
        }
        else
        {
            Debug.LogWarning("total money text not assigned in UIManager.");
        }
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
            Debug.LogWarning("npc prefab or spawn location not assigned in UIManager.");
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


}
