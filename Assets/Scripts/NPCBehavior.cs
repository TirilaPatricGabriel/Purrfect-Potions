using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    public float speed = 5f;
    private List<GameObject> waypoints;
    private List<GameObject> lastWaypoints; 
    private GameObject firstWaypoint;
    private GameObject currentLastWaypoint = null; // last waypoint occupied by this NPC
    private static Dictionary<GameObject, bool> lastWaypointStatus = new Dictionary<GameObject, bool>();

    private int currentWaypointIndex = 0;
    private bool movingToLastWaypoint = true;
    private bool returningToFirstWaypoint = false;
    private bool goingToFinalLastWaypoint = false;
    private bool orderPlaced = false; // track if order placed
    private Animator animator;

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
        { "Potion_2+Potion_3", ("Potion_6", 20f) },
        { "Potion_4+Potion_6", ("Potion_7", 30f) }
    };

    private Quaternion lastRotation;

    void Start()
    {
        animator = GetComponent<Animator>();

        // sort waypoints
        waypoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Waypoint"));
        waypoints.Sort((a, b) => a.transform.position.z.CompareTo(b.transform.position.z));

        firstWaypoint = GameObject.FindGameObjectWithTag("FirstWaypoint");

        // find all last waypoints
        lastWaypoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("LastWaypoint"));
        foreach (GameObject lastWaypoint in lastWaypoints)
        {
            if (!lastWaypointStatus.ContainsKey(lastWaypoint))
            {
                lastWaypointStatus.Add(lastWaypoint, false); // all last waypoints unoccupied
            }
        }

        lastRotation = transform.rotation;
    }

    void Update()
    {
        // freeze NPCs if all last waypoints are occupied
        if (movingToLastWaypoint && AllLastWaypointsOccupied())
        {
            animator.SetBool("isWalking", false);
            return;
        }

        if (movingToLastWaypoint)
        {
            if (!goingToFinalLastWaypoint)
            {
                MoveTowardsWaypoint(currentWaypointIndex);

                // NPC has reached the last normal waypoint
                if (currentWaypointIndex >= waypoints.Count - 1)
                {
                    goingToFinalLastWaypoint = true;
                    GameObject availableLastWaypoint = GetAvailableLastWaypoint();
                    if (availableLastWaypoint != null)
                    {
                        currentLastWaypoint = availableLastWaypoint;
                        lastWaypointStatus[availableLastWaypoint] = true; // occ. the waypoint
                    }
                }
            }
            else if (currentLastWaypoint != null)
            {
                MoveTowardsTarget(currentLastWaypoint.transform.position);
                if (Vector3.Distance(transform.position, currentLastWaypoint.transform.position) < 0.1f && !orderPlaced)
                {
                    StartCoroutine(HandleOrderPlacement());
                }
            }
        }
        else if (returningToFirstWaypoint)
        {
            MoveBackThroughWaypoints();
        }
    }

    private IEnumerator HandleOrderPlacement()
    {
        orderPlaced = true;
        animator.SetBool("isWalking", false);

        var (orderText, price) = AddRandomOrder();
        yield return new WaitForSeconds(3f); 

        CompleteOrder(orderText, price);
        RemoveOrderFromDisplay();
    }

    private (string, float) AddRandomOrder()
    {
        if (customerOrderManager == null) return (string.Empty, 0f);

        List<KeyValuePair<string, (string, float)>> recipes = new List<KeyValuePair<string, (string, float)>>(potionRecipes);
        KeyValuePair<string, (string, float)> randomRecipe = recipes[Random.Range(0, recipes.Count)];

        (string result, float price) = randomRecipe.Value;

        string[] components = randomRecipe.Key.Split('+');
        string orderText = $"{result} ({components[0]} + {components[1]})";

        customerOrderManager.AddOrder(orderText);
        return (orderText, price);
    }


    private void CompleteOrder(string orderText, float price)
    {
        SaveCompletedOrder(orderText, price);

        // free the last waypoint and prepare to return
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
        Debug.Log($"Order Completed: {orderText} | Price: {price}");
    }

    private void RemoveOrderFromDisplay()
    {
        if (customerOrderManager != null)
        {
            customerOrderManager.RemoveLastOrder();
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
        MoveTowardsTarget(waypoints[waypointIndex].transform.position);

        if (Vector3.Distance(transform.position, waypoints[waypointIndex].transform.position) < 0.1f)
        {
            currentWaypointIndex = Mathf.Min(currentWaypointIndex + 1, waypoints.Count - 1);
        }
    }

    void MoveBackThroughWaypoints()
    {
        MoveTowardsTarget(waypoints[currentWaypointIndex].transform.position);

        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].transform.position) < 0.1f)
        {
            currentWaypointIndex = Mathf.Max(currentWaypointIndex - 1, 0);
            if (currentWaypointIndex == 0)
            {
                returningToFirstWaypoint = false;
                movingToLastWaypoint = true; // ready for another cycle
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
}
