using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPotionHandler : MonoBehaviour
{
    public float interactionRange = 2.0f;
    public Transform holdPosition;
    private GameObject heldPotion;
    private int originalLayer;
    private bool canPickup = true;         

    public GameObject potion_4Prefab;
    public GameObject potion_5Prefab;
    public GameObject potion_6Prefab;
    public GameObject potion_7Prefab;

    public KeyCode interactKey = KeyCode.E; // default will be E

    private static HashSet<GameObject> takenPotions = new HashSet<GameObject>(); 

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
        Transform closestPotionPlace = null;
        float closestDistance = Mathf.Infinity;
        GameObject potionOnTable = null;

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

                        foreach (Transform child in potionPlace)
                        {
                            if ((child.CompareTag("Potion") || child.CompareTag("Potion_2") ||
                                child.CompareTag("Potion_3") || child.CompareTag("Potion_4") ||
                                child.CompareTag("Potion_5") || child.CompareTag("Potion_6") ||
                                child.CompareTag("Potion_7")) && !takenPotions.Contains(child.gameObject))
                            {
                                potionOnTable = child.gameObject;
                                break;
                            }
                        }
                    }
                }
            }
        }

        if (potionOnTable != null)
        {
            Debug.Log("picking up potion from the table: " + potionOnTable.name);
            PickUpPotion(potionOnTable);
        }
        else
        {
            TryPickupPotion();
        }
    }

    void TryPickupPotion()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);

        foreach (var collider in hitColliders)
        {
            if ((collider.CompareTag("Potion") || collider.CompareTag("Potion_2") ||
                collider.CompareTag("Potion_3") || collider.CompareTag("Potion_4") ||
                collider.CompareTag("Potion_5") || collider.CompareTag("Potion_6") ||
                collider.CompareTag("Potion_7")) && !takenPotions.Contains(collider.gameObject))
            {
                PickUpPotion(collider.gameObject);
                break;
            }
        }
    }

    private void PickUpPotion(GameObject potion)
    {
        heldPotion = potion;
        heldPotion.transform.position = holdPosition.position;
        heldPotion.transform.SetParent(holdPosition, true);

        originalLayer = heldPotion.layer;
        heldPotion.layer = LayerMask.NameToLayer("IgnorePickup");

        // disable physics
        Rigidbody potionRb = heldPotion.GetComponent<Rigidbody>();
        if (potionRb != null)
        {
            potionRb.isKinematic = true;
        }

        Debug.Log("picked up potion: " + heldPotion.name);
        canPickup = false; // prevent another pickup
        StartCoroutine(ResetPickupCooldown());
    }

    private IEnumerator ResetPickupCooldown()
    {
        yield return new WaitForSeconds(1f);
        canPickup = true;
    }

    void PlacePotionOnTableOrCombine()
    {
        // check if there is a nearby NPC with an active order
        NPCBehavior npc = FindNearbyNPCWithActiveOrder();
        Debug.Log("AFTER TRYING TO FIND NPC WITH ACTIVE ORDER");

        if (npc != null && npc.CheckIfOrderCompleted(heldPotion))
        {
            Debug.Log("order completed by placing the correct potion!");
            Destroy(heldPotion);
            return; // order completed
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
        Transform closestPotionPlace = null;
        float closestDistance = Mathf.Infinity;

        bool tableFound = false;
        GameObject potionOnTable = null;

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

                        foreach (Transform child in potionPlace)
                        {
                            if (child.CompareTag("Potion") || child.CompareTag("Potion_2") ||
                                child.CompareTag("Potion_3") || child.CompareTag("Potion_4") ||
                                child.CompareTag("Potion_5") || child.CompareTag("Potion_6") ||
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

        Destroy(heldPotion);
        Destroy(potionOnTable);

        newPotion.transform.position = potionPlace.position;
        newPotion.transform.rotation = potionPlace.rotation;
        newPotion.transform.SetParent(potionPlace, true);
    }

    void PlacePotionOnTable(Transform closestPotionPlace)
    {
        heldPotion.transform.position = closestPotionPlace.position;
        heldPotion.transform.rotation = closestPotionPlace.rotation;

        heldPotion.transform.SetParent(closestPotionPlace, true);

        Rigidbody potionRb = heldPotion.GetComponent<Rigidbody>();
        if (potionRb != null)
        {
            potionRb.isKinematic = false;
        }

        DropPotion(1);
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
                potionRb.isKinematic = false;
            }

            heldPotion.layer = originalLayer;

            takenPotions.Remove(heldPotion); // mark potion not taken
            Debug.Log("dropped potion: " + heldPotion.name);
            heldPotion = null;
        }
    }

    NPCBehavior FindNearbyNPCWithActiveOrder()
    {
        NPCBehavior[] npcs = FindObjectsOfType<NPCBehavior>();

        foreach (var npc in npcs)
        {
            // calculate distance to see if nearby
            float distanceToNPC = Vector3.Distance(transform.position, npc.transform.position);

            if (npc != null && npc.OrderPlaced && distanceToNPC <= interactionRange)
            {
                return npc;  
            }
        }

        return null; 
    }

}
