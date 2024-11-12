using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    public float speed = 5f; // Movement speed
    private List<GameObject> waypoints; // List of waypoints
    private GameObject currentTarget; // Current target waypoint
    private HashSet<GameObject> visitedWaypoints; // Keep track of visited waypoints

    void Start()
    {
        // Find all game objects tagged as "Waypoint" and store them in a list
        waypoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Waypoint"));
        visitedWaypoints = new HashSet<GameObject>();

        // Find and set the initial target waypoint
        SetNextTarget();
    }

    void Update()
    {
        if (currentTarget != null)
        {
            // Move towards the current target waypoint
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, step);

            // Check if the NPC has reached the current target waypoint
            if (Vector3.Distance(transform.position, currentTarget.transform.position) < 0.1f)
            {
                // Mark the waypoint as visited and set the next target
                visitedWaypoints.Add(currentTarget);
                SetNextTarget();
            }
        }
    }

    void SetNextTarget()
    {
        GameObject nearestWaypoint = null;
        float nearestDistance = Mathf.Infinity;

        // Loop through all waypoints to find the nearest unvisited one
        foreach (GameObject waypoint in waypoints)
        {
            if (!visitedWaypoints.Contains(waypoint))
            {
                float distance = Vector3.Distance(transform.position, waypoint.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestWaypoint = waypoint;
                }
            }
        }

        // Set the nearest unvisited waypoint as the new target
        currentTarget = nearestWaypoint;
    }
}
