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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldPotion == null && canPickup)
            {
                TryPickupPotion();
            }
            else if (heldPotion != null)
            {
                PlacePotionOnTableOrDrop();
            }
        }
    }

    void TryPickupPotion()
    {
        // Find all colliders within interactionRange
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);

        foreach (var collider in hitColliders)
        {
            // check for "Potion"
            if (collider.CompareTag("Potion"))
            {
                // pick up
                heldPotion = collider.gameObject;
                heldPotion.transform.position = holdPosition.position;
                heldPotion.transform.parent = holdPosition;  // Attach to hold position

                originalLayer = heldPotion.layer;
                heldPotion.layer = LayerMask.NameToLayer("IgnorePickup");

                // disable potion physics
                Rigidbody potionRb = heldPotion.GetComponent<Rigidbody>();
                if (potionRb != null)
                {
                    potionRb.isKinematic = true;
                }

                Debug.Log("Picked up potion: " + heldPotion.name);
                break;
            }
        }
    }

    void PlacePotionOnTableOrDrop()
    {
        // Find all colliders within interactionRange
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
        Transform closestPotionPlace = null;
        float closestDistance = Mathf.Infinity;

        bool tableFound = false;

        // find a table
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Table"))
            {
                // find potionplace (child of the found table)
                Transform potionPlace = collider.transform.Find("PotionPlace");
                if (potionPlace != null)
                {
                    float distance = Vector3.Distance(transform.position, potionPlace.position);

                    // always check for closest potionPlace 
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPotionPlace = potionPlace;
                        tableFound = true;  
                    }
                }
            }
        }

        // place potion on the closest table
        if (tableFound && closestPotionPlace != null)
        {
            Debug.Log("Placing potion on the closest table at: " + closestPotionPlace.position);

            heldPotion.transform.position = closestPotionPlace.position;
            heldPotion.transform.rotation = closestPotionPlace.rotation; 

            DropPotion();  // drop potion from player onto table

            Debug.Log("Potion placed successfully on the closest table.");
        }
        else
        {
            // no table found, drop the potion instead
            Debug.Log("No table found nearby, dropping the potion.");
            DropPotion();
        }
    }

    void DropPotion()
    {
        if (heldPotion != null)
        {
            // detach the potion and enable physics again
            heldPotion.transform.parent = null;
            Rigidbody potionRb = heldPotion.GetComponent<Rigidbody>();
            if (potionRb != null)
            {
                potionRb.isKinematic = false;  // Enable physics
            }

            // reset layer of potion
            heldPotion.layer = originalLayer;

            Debug.Log("Dropped potion: " + heldPotion.name);
            heldPotion = null; 

            // pickup cooldown
            StartCoroutine(PickupCooldown());
        }
    }

    private IEnumerator PickupCooldown()
    {
        canPickup = false;  
        yield return new WaitForSeconds(0.5f); 
        canPickup = true;  
    }
}
