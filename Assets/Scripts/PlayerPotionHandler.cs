using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPotionHandler : MonoBehaviour
{
    public float interactionRange = 2.0f;  
    public Transform holdPosition;          
    private GameObject heldPotion;          
    private int originalLayer;              // Original layer of potion before player gets it
    private bool canPickup = true;          // Cooldown for potion pickup

    // Prefabs for combinations
    public GameObject potion_4Prefab;
    public GameObject potion_5Prefab;
    public GameObject potion_6Prefab;
    public GameObject potion_7Prefab;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldPotion == null && canPickup)
            {
                TryPickupPotionOrTablePotion();
            }
            else if (heldPotion != null)
            {
                PlacePotionOnTableOrCombine(); 
            }
        }
    }

    void TryPickupPotionOrTablePotion()
    {
        // Find all colliders within range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
        Transform closestPotionPlace = null;
        float closestDistance = Mathf.Infinity;
        GameObject potionOnTable = null;

        // Try to find nearest table with potion
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Table"))
            {
                Transform potionPlace = collider.transform.Find("PotionPlace");
                if (potionPlace != null)
                {
                    float distance = Vector3.Distance(transform.position, potionPlace.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPotionPlace = potionPlace;

                        // Check if a potion is on the table
                        foreach (Transform child in potionPlace)
                        {
                            if (child.CompareTag("Potion") ||
                                child.CompareTag("Potion_2") ||
                                child.CompareTag("Potion_3") ||
                                child.CompareTag("Potion_4") ||
                                child.CompareTag("Potion_5") ||
                                child.CompareTag("Potion_6") ||
                                child.CompareTag("Potion_7"))
                            {
                                potionOnTable = child.gameObject;
                                break;
                            }
                        }
                    }
                }
            }
        }

        // If a potion is found on the table, pick it up
        if (potionOnTable != null)
        {
            Debug.Log("Picking up potion from the table: " + potionOnTable.name);
            PickUpPotion(potionOnTable);
        }
        else
        {
            // Try to pick up a potion from the world
            TryPickupPotion();
        }
    }

    void TryPickupPotion()
    {
        // Find all colliders within interactionRange
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);

        foreach (var collider in hitColliders)
        {
            // Check for any potions
            if (collider.CompareTag("Potion") ||
                collider.CompareTag("Potion_2") ||
                collider.CompareTag("Potion_3") ||
                collider.CompareTag("Potion_4") ||
                collider.CompareTag("Potion_5") ||
                collider.CompareTag("Potion_6") ||
                collider.CompareTag("Potion_7"))
            {
                // Pick up
                PickUpPotion(collider.gameObject);
                break;
            }
        }
    }

    private void PickUpPotion(GameObject potion)
    {
        heldPotion = potion;
        heldPotion.transform.position = holdPosition.position;
        heldPotion.transform.SetParent(holdPosition, true); // With maintain world position

        originalLayer = heldPotion.layer;
        heldPotion.layer = LayerMask.NameToLayer("IgnorePickup");

        // Disable potion physics
        Rigidbody potionRb = heldPotion.GetComponent<Rigidbody>();
        if (potionRb != null)
        {
            potionRb.isKinematic = true;
        }

        Debug.Log("Picked up potion: " + heldPotion.name);
        canPickup = false;  // Prevent further pickups until reset
        StartCoroutine(ResetPickupCooldown());
    }

    private IEnumerator ResetPickupCooldown()
    {
        yield return new WaitForSeconds(1f); 
        canPickup = true; 
    }

    void PlacePotionOnTableOrCombine()
    {
        // Find all colliders within interactionRange
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
        Transform closestPotionPlace = null;
        float closestDistance = Mathf.Infinity;

        bool tableFound = false;
        GameObject potionOnTable = null;  // Store potion found on the table

        // Find the closest table 
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Table"))
            {
                Transform potionPlace = collider.transform.Find("PotionPlace");
                if (potionPlace != null)
                {
                    float distance = Vector3.Distance(transform.position, potionPlace.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPotionPlace = potionPlace;
                        tableFound = true;

                        // Check for potions already on the table
                        foreach (Transform child in potionPlace)
                        {
                            if (child.CompareTag("Potion") ||
                                child.CompareTag("Potion_2") ||
                                child.CompareTag("Potion_3") ||
                                child.CompareTag("Potion_4") ||
                                child.CompareTag("Potion_5") ||
                                child.CompareTag("Potion_6") ||
                                child.CompareTag("Potion_7"))
                            {
                                potionOnTable = child.gameObject;
                                break;  
                            }
                        }
                    }
                }
            }
        }

        if (tableFound)
        {
            if (potionOnTable != null)
            {
                CombinePotions(potionOnTable, closestPotionPlace);
            }
            else if (closestPotionPlace != null)
            {
                PlacePotionOnTable(closestPotionPlace);
            }
        }
        else
        {
            DropPotion(0);
        }
    }

    void CombinePotions(GameObject potionOnTable, Transform potionPlace)
    {
        string heldTag = heldPotion.tag;
        string tableTag = potionOnTable.tag;
        GameObject newPotion = null;

        Debug.Log(heldTag);
        Debug.Log(tableTag);

        if ((string.Equals(heldTag, "Potion") && string.Equals(tableTag, "Potion_2")) ||
            (string.Equals(heldTag, "Potion_2") && string.Equals(tableTag, "Potion")))
        {
            newPotion = Instantiate(potion_4Prefab, potionPlace.position, Quaternion.identity);
            newPotion.tag = "Potion_4";
        }
        else if ((string.Equals(heldTag, "Potion") && string.Equals(tableTag, "Potion_3")) ||
                 (string.Equals(heldTag, "Potion_3") && string.Equals(tableTag, "Potion")))
        {
            newPotion = Instantiate(potion_5Prefab, potionPlace.position, Quaternion.identity);
            newPotion.tag = "Potion_5";
        }
        else if ((string.Equals(heldTag, "Potion_2") && string.Equals(tableTag, "Potion_3")) ||
                 (string.Equals(heldTag, "Potion_3") && string.Equals(tableTag, "Potion_2")))
        {
            newPotion = Instantiate(potion_6Prefab, potionPlace.position, Quaternion.identity);
            newPotion.tag = "Potion_6";
        }
        else if ((string.Equals(heldTag, "Potion_4") && string.Equals(tableTag, "Potion_6")) ||
                 (string.Equals(heldTag, "Potion_6") && string.Equals(tableTag, "Potion_4")))
        {
            newPotion = Instantiate(potion_7Prefab, potionPlace.position, Quaternion.identity);
            newPotion.tag = "Potion_7";
        }
        else
        {
            Debug.Log("No valid potion combination found. Throwing the potion.");
            DropPotion(0);
            return;
        }

        // Destroy the original potions
        Destroy(heldPotion);  
        Destroy(potionOnTable);  

        // Place the new potion on the table and parent it
        newPotion.transform.position = potionPlace.position;  
        newPotion.transform.rotation = potionPlace.rotation;  
        newPotion.transform.SetParent(potionPlace, true); 
    }

    void PlacePotionOnTable(Transform closestPotionPlace)
    {
        heldPotion.transform.position = closestPotionPlace.position;
        heldPotion.transform.rotation = closestPotionPlace.rotation;

        // Set the potion as a child of the PotionPlace
        heldPotion.transform.SetParent(closestPotionPlace, true); 

        // Reset rigidbody properties if needed
        Rigidbody potionRb = heldPotion.GetComponent<Rigidbody>();
        if (potionRb != null)
        {
            potionRb.isKinematic = false;  // Enable physics
        }

        Debug.Log("Placed " + heldPotion.name + " under " + closestPotionPlace.name);

        DropPotion(1);  // Call DropPotion to reset heldPotion bool
    }

    void DropPotion(int dontResetParent)
    {
        if (heldPotion != null)
        {
            if (dontResetParent == 0)
            {
                heldPotion.transform.parent = null; 
            }
            Rigidbody potionRb = heldPotion.GetComponent<Rigidbody>();
            if (potionRb != null)
            {
                potionRb.isKinematic = false;  // Enable physics
            }

            // Reset layer to original
            heldPotion.layer = originalLayer;

            Debug.Log("Dropped potion: " + heldPotion.name);
            heldPotion = null; // Clear held potion reference
        }
    }
}
