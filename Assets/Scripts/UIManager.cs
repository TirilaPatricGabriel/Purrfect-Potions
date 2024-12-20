using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI totalMoneyText;
    public static float totalMoney = 0f;

    public void UpdateTotalMoneyText()
    {
        if (totalMoneyText != null)
        {
            totalMoneyText.text = $"Total Money: ${totalMoney}";
        }
        else
        {
            Debug.LogWarning("Total Money Text not assigned in UIManager.");
        }
    }

    public static void AddMoney(float amount)
    {
        totalMoney += amount;
    }
}
