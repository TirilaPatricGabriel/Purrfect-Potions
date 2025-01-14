using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

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

    // anger
    private float waitTime = 0f;
    private float angerThreshold = 20f;
    private const float angerPenalty = 10f; // $10

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

    private static Dictionary<string, (string result, float price)> potionRecipes;
    private Quaternion lastRotation;
    private string expectedPotionTag;

    void Start()
    {
        InitializePotionRecipes();

        uiManager = FindObjectOfType<UIManager>();

        angerThreshold = Random.Range(20f, 50f);

        if (uiManager == null)
        {
            Debug.LogError("UIManager not found in the scene!");
        }

        customerOrderManager = FindObjectOfType<CustomerOrderManager>();

        if (customerOrderManager == null)
        {
            Debug.LogError("CustomerOrderManager not found in the scene!");
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
            }
        }

        lastRotation = transform.rotation;
    }

    void InitializePotionRecipes()
    {
        potionRecipes = new Dictionary<string, (string result, float price)>();

        for (int i = 1; i <= 14; i++)
        {
            string resultTag = i == 1 ? "Potion" : $"Potion_{i}";

            float price = i * 5f;

            if (!potionRecipes.ContainsKey(resultTag))
            {
                potionRecipes[resultTag] = (resultTag, price);
            }
        }
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

                if (Vector3.Distance(transform.position, currentLastWaypoint.transform.position) < 0.1f)
                {
                    waitTime += Time.deltaTime;
                    if (waitTime >= angerThreshold)
                    {
                        CancelOrder();
                    }
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
    }

    private (string, float) AddRandomOrder()
    {
        if (customerOrderManager == null) return (string.Empty, 0f);

        List<KeyValuePair<string, (string, float)>> recipes = new List<KeyValuePair<string, (string, float)>>(potionRecipes);
        KeyValuePair<string, (string, float)> randomRecipe = recipes[Random.Range(0, recipes.Count)];

        (string result, float price) = randomRecipe.Value;

        //string[] components = randomRecipe.Key.Split('+');
        string orderText = $"{result}";  

        customerOrderManager.AddOrder(orderText, price);  

        expectedPotionTag = result;  

        orderPlaced = true;

        return (orderText, price);
    }


    public bool CheckIfOrderCompleted(GameObject droppedPotion)
    {
        if (orderPlaced && droppedPotion.tag == expectedPotionTag)
        {
            foreach (var recipe in potionRecipes.Values)
            {
                if (droppedPotion.tag == recipe.result)
                {
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

        if (uiManager)
        {
            UIManager.totalMoney += price; 
            uiManager.UpdateTotalMoneyText();
            uiManager.OrderCompletedOrCanceled();
        }

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

    private void CancelOrder()
    {
        if (customerOrderManager != null)
        {
            customerOrderManager.RemoveOrder(expectedPotionTag);
        }

        if (uiManager)
        {
            UIManager.totalMoney -= angerPenalty;  
            uiManager.UpdateTotalMoneyText();
            uiManager.OrderCompletedOrCanceled();
        }

        if (currentLastWaypoint != null)
        {
            lastWaypointStatus[currentLastWaypoint] = false;
            currentLastWaypoint = null;
        }

        returningToFirstWaypoint = true;
        movingToLastWaypoint = false;
        orderPlaced = false;
    }
}
