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
}
