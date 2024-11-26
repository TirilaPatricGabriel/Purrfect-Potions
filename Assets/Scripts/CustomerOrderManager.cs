using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CustomerOrderManager : MonoBehaviour
{
    public GameObject orderBoxPrefab; 
    public Transform ordersContainer; 

    private List<GameObject> activeOrders = new List<GameObject>(); 
    private const int MaxOrders = 3; 

    void Start()
    {
        CustomerOrderManager customerOrderManager = GetComponent<CustomerOrderManager>();
        customerOrderManager.AddOrder("Potion + Herb");
        customerOrderManager.AddOrder("Elixir + Crystal");
        customerOrderManager.AddOrder("Scroll + Ink");
        customerOrderManager.AddOrder("Bonus Order!");
    }


    // Add a new order to the list

    public void AddOrder(string orderText)
    {
        if (activeOrders.Count >= MaxOrders)
        {
            RemoveOldestOrder();
        }

        GameObject newOrderBox = Instantiate(orderBoxPrefab, ordersContainer);

        TextMeshProUGUI orderTextComponent = newOrderBox.GetComponentInChildren<TextMeshProUGUI>();
        if (orderTextComponent == null)
        {
            Debug.LogError("No TextMeshProUGUI component found in OrderBox prefab!");
            return;
        }

        orderTextComponent.text = orderText;

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

}
