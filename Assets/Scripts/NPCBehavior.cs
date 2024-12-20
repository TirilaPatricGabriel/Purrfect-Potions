using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    public float speed = 5f;
    private List<GameObject> waypoints;
    private List<GameObject> lastWaypoints; // list of waypoints
    private GameObject firstWaypoint;
    private GameObject currentLastWaypoint = null; // lastwaypoint occupied
    private static Dictionary<GameObject, bool> lastWaypointStatus = new Dictionary<GameObject, bool>();

    private int currentWaypointIndex = 0;
    private bool movingToLastWaypoint = true;
    private bool returningToFirstWaypoint = false;
    private bool goingToFinalLastWaypoint = false;
    private bool orderPlaced = false; // this npc placed order or not
    private Animator animator;

    public UIManager uiManager;
    public float totalMoney = 0f;

    public CustomerOrderManager customerOrderManager;

    public struct CompletedOrder
    {
        public string OrderText;
        public float Price;

        public CompletedOrder(string orderText, float price)
        {
            OrderText = orderText;
            Price = price;
        }
    }

    private static List<CompletedOrder> completedOrders = new List<CompletedOrder>();

    private static Dictionary<string, (string result, float price)> potionRecipes = new Dictionary<string, (string, float)>
    {
        { "Potion+Potion_2", ("Potion_4", 10f) },
        { "Potion+Potion_3", ("Potion_5", 15f) },
        { "Potion_2+Potion_3", ("Potion", 20f) },
        { "Potion_4+Potion_6", ("Potion_2", 30f) }
    };

    private Quaternion lastRotation;
    private string expectedPotionTag;

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();

        if (uiManager == null)
        {
            Debug.LogError("UIManager not found in the scene!");
        }

        animator = GetComponent<Animator>();

        // sort by x waypoints
        waypoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Waypoint"));
        waypoints.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        firstWaypoint = GameObject.FindGameObjectWithTag("FirstWaypoint");

        // sort by z the lastwaypoints
        lastWaypoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("LastWaypoint"));
        lastWaypoints.Sort((a, b) => b.transform.position.z.CompareTo(a.transform.position.z));

        foreach (GameObject lastWaypoint in lastWaypoints)
        {
            if (!lastWaypointStatus.ContainsKey(lastWaypoint))
            {
                lastWaypointStatus.Add(lastWaypoint, false); // unoccupied
                Debug.Log($"Initialized LastWaypoint: {lastWaypoint.name} at Position: {lastWaypoint.transform.position}");
            }
        }

        lastRotation = transform.rotation;
    }

    void Update()
    {
        // freeze
        if (movingToLastWaypoint && currentLastWaypoint == null && AllLastWaypointsOccupied())
        {
            animator.SetBool("isWalking", false); 
            return;
        }

        if (movingToLastWaypoint)
        {
            if (!goingToFinalLastWaypoint)
            {
                MoveTowardsWaypoint(currentWaypointIndex);

                if (currentWaypointIndex == waypoints.Count - 1 &&
                    Vector3.Distance(transform.position, waypoints[currentWaypointIndex].transform.position) < 0.5f)
                {
                    goingToFinalLastWaypoint = true;

                    // wait for available lastwaypoint
                    GameObject availableLastWaypoint = GetAvailableLastWaypoint();
                    if (availableLastWaypoint != null)
                    {
                        currentLastWaypoint = availableLastWaypoint;
                        lastWaypointStatus[availableLastWaypoint] = true; // occupy
                        Debug.Log($"Assigned NPC to LastWaypoint: {currentLastWaypoint.name}");
                    }
                    else
                    {
                        // wait for lastwaypoint to be available
                        animator.SetBool("isWalking", false);
                        return;
                    }
                }
            }
            else if (currentLastWaypoint != null)
            {
                MoveTowardsTarget(currentLastWaypoint.transform.position);
                if (Vector3.Distance(transform.position, currentLastWaypoint.transform.position) < 0.1f && !orderPlaced)
                {
                    HandleOrderPlacement();
                }
            }
        }
        else if (returningToFirstWaypoint)
        {
            MoveBackThroughWaypoints();
        }
    }

    private void HandleOrderPlacement()
    {
        orderPlaced = true;
        animator.SetBool("isWalking", false);

        var (orderText, price) = AddRandomOrder();
        Debug.Log($"Order Placed: {orderText} | Price: {price}");
    }

    private (string, float) AddRandomOrder()
    {
        if (customerOrderManager == null) return (string.Empty, 0f);

        List<KeyValuePair<string, (string, float)>> recipes = new List<KeyValuePair<string, (string, float)>>(potionRecipes);
        KeyValuePair<string, (string, float)> randomRecipe = recipes[Random.Range(0, recipes.Count)];

        (string result, float price) = randomRecipe.Value;

        string[] components = randomRecipe.Key.Split('+');
        string orderText = $"{result} ({components[0]} + {components[1]})";  

        customerOrderManager.AddOrder(orderText, price);  

        expectedPotionTag = result;  

        orderPlaced = true;

        return (orderText, price);
    }


    public bool CheckIfOrderCompleted(GameObject droppedPotion)
    {
        if (orderPlaced && droppedPotion.tag == expectedPotionTag)
        {
            Debug.Log("Correct potion dropped! Completing order...");

            Debug.Log("EXPECTED POTION TAG:" + expectedPotionTag);

            foreach (var recipe in potionRecipes.Values)
            {
                if (droppedPotion.tag == recipe.result)
                {
                    Debug.Log("Correct potion dropped! Completing order...");
                    Debug.Log("EXPECTED POTION RESULT: " + recipe.result);

                    CompleteOrder(droppedPotion.name, droppedPotion.tag, recipe.price);
                    return true;  
                }
            }
        }
        return false;  
    }

    private void CompleteOrder(string orderText, string potionTag, float price)
    {
        SaveCompletedOrder(orderText, price);

        if (customerOrderManager != null)
        {
            customerOrderManager.RemoveOrder(potionTag);  
        }

        if (currentLastWaypoint != null)
        {
            Debug.Log($"Freeing LastWaypoint: {currentLastWaypoint.name}");
            lastWaypointStatus[currentLastWaypoint] = false;
            currentLastWaypoint = null;
        }

        returningToFirstWaypoint = true;
        movingToLastWaypoint = false;
        orderPlaced = false;

        currentWaypointIndex = waypoints.Count - 1;
    }

    private void SaveCompletedOrder(string orderText, float price)
    {
        completedOrders.Add(new CompletedOrder(orderText, price));  
        Debug.Log($"Order Completed: {orderText} | Price: {price}");

        PrintCompletedOrdersAndTotalMoney();
    }

    private void PrintCompletedOrdersAndTotalMoney()
    {
        totalMoney = 0f;

        foreach (var completedOrder in completedOrders)
        {
            totalMoney += completedOrder.Price;
        }

        if (uiManager != null)
        {
            uiManager.UpdateTotalMoneyText(totalMoney);  
        }

        Debug.Log($"Total Money Earned So Far: {totalMoney}");
    }

    GameObject GetAvailableLastWaypoint()
    {
        foreach (var waypoint in lastWaypoints)
        {
            if (!lastWaypointStatus[waypoint])
            {
                return waypoint; 
            }
        }
        return null; 
    }

    bool AllLastWaypointsOccupied()
    {
        foreach (var status in lastWaypointStatus.Values)
        {
            if (!status) return false;
        }
        return true;
    }

    void MoveTowardsWaypoint(int waypointIndex)
    {
        if (!IsIndexValid(waypointIndex)) return;

        MoveTowardsTarget(waypoints[waypointIndex].transform.position);

        if (Vector3.Distance(transform.position, waypoints[waypointIndex].transform.position) < 0.5f)
        {
            currentWaypointIndex = Mathf.Min(currentWaypointIndex + 1, waypoints.Count - 1);
        }
    }

    void MoveBackThroughWaypoints()
    {
        if (!IsIndexValid(currentWaypointIndex)) return;

        MoveTowardsTarget(waypoints[currentWaypointIndex].transform.position);

        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].transform.position) < 0.1f)
        {
            currentWaypointIndex--;
            if (currentWaypointIndex < 0)
            {
                returningToFirstWaypoint = false;
                movingToLastWaypoint = true;
                gameObject.SetActive(false);
            }
        }
    }

    void MoveTowardsTarget(Vector3 targetPosition)
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        bool isMoving = Vector3.Distance(transform.position, targetPosition) > 0.1f;
        animator.SetBool("isWalking", isMoving);

        if (isMoving)
        {
            RotateTowards(targetPosition);
        }
    }

    void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction.magnitude > 0.1f)
        {
            lastRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lastRotation, Time.deltaTime * 10f);
        }
    }

    private bool IsIndexValid(int index)
    {
        return index >= 0 && index < waypoints.Count;
    }

    public bool OrderPlaced
    {
        get { return orderPlaced; }
        set { orderPlaced = value; }
    }
}
