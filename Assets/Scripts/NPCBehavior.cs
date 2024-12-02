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
    private Animator animator;

    public CustomerOrderManager customerOrderManager;

    private static Dictionary<string, string> potionRecipes = new Dictionary<string, string>
    {
        { "Potion+Potion_2", "Potion_4" },
        { "Potion+Potion_3", "Potion_5" },
        { "Potion_2+Potion_3", "Potion_6" },
        { "Potion_4+Potion_6", "Potion_7" }
    };

    private Quaternion lastRotation; // Variabilă pentru a stoca ultima rotație a NPC-ului

    void Start()
    {
        animator = GetComponent<Animator>();

        // Sortăm toate waypoint-urile
        waypoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Waypoint"));
        waypoints.Sort((a, b) => a.transform.position.z.CompareTo(b.transform.position.z)); // Sortare după axa Z

        firstWaypoint = GameObject.FindGameObjectWithTag("FirstWaypoint");
        lastWaypoint = GameObject.FindGameObjectWithTag("LastWaypoint");

        // Asigurăm că FirstWaypoint este la începutul listei
        if (firstWaypoint != null && waypoints[0] != firstWaypoint)
        {
            waypoints.Remove(firstWaypoint);
            waypoints.Insert(0, firstWaypoint);
        }

        // Asigurăm că LastWaypoint este la sfârșitul listei
        if (lastWaypoint != null && waypoints[waypoints.Count - 1] != lastWaypoint)
        {
            waypoints.Remove(lastWaypoint);
            waypoints.Add(lastWaypoint);
        }

        // Setăm rotația inițială
        lastRotation = transform.rotation;
    }

    void Update()
    {
        // Freeze doar pentru NPC-urile care merg spre ultimul waypoint
        if (movingToLastWaypoint && isLastWaypointOccupied && npcAtLastWaypoint != gameObject)
        {
            animator.SetBool("isWalking", false); // Oprește animația
            return;
        }

        if (movingToLastWaypoint)
        {
            if (!IsIndexValid(currentWaypointIndex)) return;

            MoveTowardsWaypoint(currentWaypointIndex);

            // Verificăm dacă s-a ajuns la ultimul waypoint
            if (waypoints[currentWaypointIndex] == lastWaypoint &&
                Vector3.Distance(transform.position, lastWaypoint.transform.position) < 0.1f)
            {
                if (!isLastWaypointOccupied)
                {
                    // Ocupăm ultimul waypoint
                    npcAtLastWaypoint = gameObject;
                    isLastWaypointOccupied = true;

                    // Adaugăm o comandă
                    AddRandomOrder();
                    StartCoroutine(WaitAtLastWaypoint());
                }
            }
        }
        else if (returningToFirstWaypoint)
        {
            MoveTowardsFirstWaypoint();

            // Verificăm dacă s-a ajuns la primul waypoint și dezactivăm NPC-ul
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

    void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Actualizează rotația doar dacă există o direcție validă
        if (direction.magnitude > 0.1f)
        {
            lastRotation = Quaternion.LookRotation(direction); // Actualizează rotația dorită
            transform.rotation = Quaternion.Slerp(transform.rotation, lastRotation, Time.deltaTime * 10f);
        }
    }


    void MoveTowardsWaypoint(int waypointIndex)
    {
        if (!IsIndexValid(waypointIndex)) return;

        float step = speed * Time.deltaTime;
        Vector3 targetPosition = waypoints[waypointIndex].transform.position;

        // Mișcă NPC-ul spre destinație
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        bool isMoving = Vector3.Distance(transform.position, targetPosition) > 0.1f;
        animator.SetBool("isWalking", isMoving);

        if (isMoving)
        {
            RotateTowards(targetPosition); // Rotim NPC-ul doar dacă se mișcă
        }
        

        // Dacă ajunge la waypoint, trece la următorul
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            if (movingToLastWaypoint)
            {
                currentWaypointIndex = Mathf.Min(currentWaypointIndex + 1, waypoints.Count - 1);
            }
            else
            {
                currentWaypointIndex = Mathf.Max(currentWaypointIndex - 1, 0);
            }

            animator.SetBool("isWalking", false); // Oprește animația
        }
    }


    void MoveTowardsFirstWaypoint()
    {
        float step = speed * Time.deltaTime;
        Vector3 targetPosition = firstWaypoint.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        bool isMoving = Vector3.Distance(transform.position, targetPosition) > 0.1f;
        animator.SetBool("isWalking", isMoving);

        if (isMoving)
        {
            RotateTowards(targetPosition);
            lastRotation = transform.rotation; // Salvează rotația curentă când NPC-ul se mișcă
        }
      
    }

    IEnumerator WaitAtLastWaypoint()
    {
        yield return new WaitForSeconds(3f);
        animator.SetBool("isWalking", false); // Oprește animația

        OrderOfNPCCompleted();
    }

    public void OrderOfNPCCompleted()
    {
        // Eliberăm ultimul waypoint și revenim la primul
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
