using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CustomerOrderManager : MonoBehaviour
{
    public GameObject orderBoxPrefab; 
    public Transform ordersContainer; 

    private List<GameObject> activeOrders = new List<GameObject>(); 
    private const int MaxOrders = 3; 

    public void AddOrder(string orderText, float price)
    {
        if (activeOrders.Count >= MaxOrders)
        {
            RemoveOldestOrder(); 
        }

        GameObject newOrderBox = Instantiate(orderBoxPrefab, ordersContainer);
        TextMeshProUGUI orderTextComponent = newOrderBox.GetComponentInChildren<TextMeshProUGUI>();

        if (orderTextComponent == null)
        {
            Debug.LogError("ERROR: no orderTextComponent found in the orderBoxPrefab!");
            return;
        }

        orderTextComponent.text = $"{orderText} - Price: ${price}"; 

        activeOrders.Add(newOrderBox);
    }


    private void RemoveOldestOrder()
    {
        if (activeOrders.Count > 0)
        {
            GameObject oldestOrder = activeOrders[0];
            activeOrders.RemoveAt(0); 
            Destroy(oldestOrder); 
        }
    }

    public void RemoveOrder(string orderText)
    {
        foreach (var orderUI in activeOrders)
        {
            TextMeshProUGUI orderTextComponent = orderUI.GetComponentInChildren<TextMeshProUGUI>();

            if (orderTextComponent != null)
            {
                string orderTextComponentText = orderTextComponent.text.Split('-')[0].Trim();

                Debug.Log("checking order: " + orderTextComponentText);
                Debug.Log("checking order text: " + orderText);

                if (orderTextComponentText == orderText)
                {
                    activeOrders.Remove(orderUI);
                    Destroy(orderUI); 
                    Debug.Log("removed Order from UI: " + orderText);
                    break;
                }
            }
        }
    }


}
