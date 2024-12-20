using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI totalMoneyText; 

    public void UpdateTotalMoneyText(float totalMoney)
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
}
