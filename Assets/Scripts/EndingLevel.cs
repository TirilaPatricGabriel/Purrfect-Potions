using TMPro;
using UnityEngine;

public class EndingLevelUI : MonoBehaviour
{
    public TextMeshProUGUI totalMoneyText;

    void Start()
    {
       
        float totalMoney = PlayerPrefs.GetFloat("TotalMoney", 0f);
       
        if (totalMoneyText != null)
        {
            totalMoneyText.text = $"Total Money Earned: ${totalMoney}";
        }
        else
        {
            Debug.LogWarning("totalMoneyText nu este asignat în EndingLevelUI.");
        }
    }
}
