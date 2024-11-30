using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    public float speed = 5f; 
    private List<GameObject> waypoints; 
    private GameObject lastWaypoint; 
    private GameObject firstWaypoint; 
    private static GameObject npcAtLastWaypoint = null; 
    private static bool isLastWaypointOccupied = false; 

    private int currentWaypointIndex = 0; 
    private bool movingToLastWaypoint = true; 
    private bool returningToFirstWaypoint = false; 

    public CustomerOrderManager customerOrderManager;

    private static Dictionary<string, string> potionRecipes = new Dictionary<string, string>
    {
        { "Potion+Potion_2", "Potion_4" },
        { "Potion+Potion_3", "Potion_5" },
        { "Potion_2+Potion_3", "Potion_6" },
        { "Potion_4+Potion_6", "Potion_7" }
    };

    void Start()
    {
        // sort all waypoints
        waypoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Waypoint"));
        waypoints.Sort((a, b) => a.transform.position.z.CompareTo(b.transform.position.z)); // Sort by Z axis

        firstWaypoint = GameObject.FindGameObjectWithTag("FirstWaypoint");
        lastWaypoint = GameObject.FindGameObjectWithTag("LastWaypoint");

        // make sure FirstWaypoint is at the start of the list
        if (firstWaypoint != null && waypoints[0] != firstWaypoint)
        {
            waypoints.Remove(firstWaypoint);
            waypoints.Insert(0, firstWaypoint);
        }

        // make sure LastWaypoint is at the end of the list
        if (lastWaypoint != null && waypoints[waypoints.Count - 1] != lastWaypoint)
        {
            waypoints.Remove(lastWaypoint);
            waypoints.Add(lastWaypoint);
        }
    }

    void Update()
    {
        // freeze only npcs moving towards last waypoint
        if (movingToLastWaypoint && isLastWaypointOccupied && npcAtLastWaypoint != gameObject)
        {
            return;
        }

        if (movingToLastWaypoint)
        {
            if (!IsIndexValid(currentWaypointIndex)) return;

            MoveTowardsWaypoint(currentWaypointIndex);

            // check if lastwaypoint reached
            if (waypoints[currentWaypointIndex] == lastWaypoint &&
                Vector3.Distance(transform.position, lastWaypoint.transform.position) < 0.1f)
            {
                if (!isLastWaypointOccupied)
                {
                    // occupy last waypoint
                    npcAtLastWaypoint = gameObject;
                    isLastWaypointOccupied = true;

                    // add order
                    AddRandomOrder();

                    StartCoroutine(WaitAtLastWaypoint());
                }
            }
        }
        else if (returningToFirstWaypoint)
        {
            MoveTowardsFirstWaypoint();

            // check if firstwaypoint reached and make customer inactive if so
            if (Vector3.Distance(transform.position, firstWaypoint.transform.position) < 0.1f)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void AddRandomOrder()
    {
        if (customerOrderManager == null) return;

        List<KeyValuePair<string, string>> recipes = new List<KeyValuePair<string, string>>(potionRecipes);
        KeyValuePair<string, string> randomRecipe = recipes[Random.Range(0, recipes.Count)];

        string[] components = randomRecipe.Key.Split('+');
        string result = randomRecipe.Value;

        string orderText = $"{result} ({components[0]} + {components[1]})";

        customerOrderManager.AddOrder(orderText);
    }

    void MoveTowardsWaypoint(int waypointIndex)
    {
        if (!IsIndexValid(waypointIndex)) return;

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, waypoints[waypointIndex].transform.position, step);

        // update index if npc reaches waypoint
        if (Vector3.Distance(transform.position, waypoints[waypointIndex].transform.position) < 0.1f)
        {
            if (movingToLastWaypoint)
            {
                currentWaypointIndex = Mathf.Min(currentWaypointIndex + 1, waypoints.Count - 1);
            }
            else
            {
                currentWaypointIndex = Mathf.Max(currentWaypointIndex - 1, 0);
            }
        }
    }

    void MoveTowardsFirstWaypoint()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, firstWaypoint.transform.position, step);
    }

    IEnumerator WaitAtLastWaypoint()
    {
        yield return new WaitForSeconds(3f);

        OrderOfNPCCompleted();
    }

    public void OrderOfNPCCompleted()
    {
        // free lastwaypoint + go to first waypoint + remove order
        if (npcAtLastWaypoint == gameObject)
        {
            isLastWaypointOccupied = false;
            npcAtLastWaypoint = null;

            if (customerOrderManager != null)
            {
                customerOrderManager.RemoveLastOrder();
            }

            returningToFirstWaypoint = true;
            movingToLastWaypoint = false;
        }
    }

    private bool IsIndexValid(int index)
    {
        return index >= 0 && index < waypoints.Count;
    }
}
